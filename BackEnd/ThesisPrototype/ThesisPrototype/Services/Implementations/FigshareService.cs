using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Converters;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Figshare;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class FigshareService : IFigshareService
    {
        private static HttpClient client = new HttpClient();

        private readonly IConfiguration configuration;

        public FigshareService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Response> PublishMultipleRepositories(List<PublishInfo> publishInfos, List<Publication> duplicates, PublishInfo bundlePublishInfo)
        {
            List<PublishingSystemPublication> publications = new List<PublishingSystemPublication>();
            List<Response> responses = new List<Response>();

            for (int i = 0; i < publishInfos.Count; i++)
            {
                FigshareMetaData figshareMetaData = publishInfos[i].metaData.ToFigshareMetaData();
                HttpResponseMessage response = await CreateArticle(figshareMetaData, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                FigshareCreateResponse figshareCreateResponse = await response.ToFigshareCreateResponse();
                int articleId = int.Parse(figshareCreateResponse.location.Split('/').Last());

                response = await CreateFile(publishInfos[i].repoName, publishInfos[i].snapshot.zippedBytes.Length, articleId, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                figshareCreateResponse = await response.ToFigshareCreateResponse();
                int fileId = int.Parse(figshareCreateResponse.location.Split('/').Last());

                response = await GetFileInfo(articleId, fileId, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                FigshareFile file = await response.ToFigshareFile();

                response = await UploadBytesToFile(publishInfos[i].snapshot.zippedBytes, file.upload_url, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                response = await CompleteFileUpload(articleId, fileId, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                publications.Add(new PublishingSystemPublication()
                {
                    publicationId = articleId.ToString(),
                    publishInfo = publishInfos[i]
                });
            }

            for (int i = 0; i < publications.Count; i++)
            {
                HttpResponseMessage response = await PublishArticle(int.Parse(publications[i].publicationId), publications[i].publishInfo.token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteArticle(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }
                else
                {
                    FigshareCreateResponse figshareCreateResponse = await response.ToFigshareCreateResponse();
                    publications[i].publicationUrl = figshareCreateResponse.location;
                }
            }

            FigshareMetaData bundleFigshareMetaData = bundlePublishInfo.metaData.ToFigshareMetaData();
            bundleFigshareMetaData.references = publications.Select(x => x.publicationUrl)
                .Union(duplicates.Select(x => x.PublicationUrl))
                .ToArray();

            HttpResponseMessage bundleResponse = await CreateArticle(bundleFigshareMetaData, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(bundleResponse);
            }

            FigshareCreateResponse figshareBundleCreateResponse = await bundleResponse.ToFigshareCreateResponse();
            int bundleArticleId = int.Parse(figshareBundleCreateResponse.location.Split('/').Last());

            bundleResponse = await PublishArticle(bundleArticleId, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                await DeleteArticle(bundleArticleId, bundlePublishInfo.token);
                return await PublishErrorResponse(bundleResponse);
            }

            return await bundleResponse.ToMultiplePublishResponse(publications, figshareBundleCreateResponse.location);
        }

        public async Task<Response> PublishRepository(byte[] repoBytes, PublishInfo publishInfo)
        {
            FigshareMetaData figshareMetaData = publishInfo.metaData.ToFigshareMetaData();
            HttpResponseMessage response = await CreateArticle(figshareMetaData, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(response);
            }

            FigshareCreateResponse figshareCreateResponse = await response.ToFigshareCreateResponse();
            int articleId = int.Parse(figshareCreateResponse.location.Split('/').Last());

            response = await CreateFile(publishInfo.repoName, repoBytes.Length, articleId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteArticle(articleId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            figshareCreateResponse = await response.ToFigshareCreateResponse();
            int fileId = int.Parse(figshareCreateResponse.location.Split('/').Last());

            response = await GetFileInfo(articleId, fileId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteArticle(articleId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            FigshareFile file = await response.ToFigshareFile();

            response = await UploadBytesToFile(repoBytes, file.upload_url, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteArticle(articleId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            response = await CompleteFileUpload(articleId, fileId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteArticle(articleId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            response = await PublishArticle(articleId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteArticle(articleId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            figshareCreateResponse = await response.ToFigshareCreateResponse();
            return new PublishResponse()
            {
                message = "Repository succesfully published.",
                publishUrl = figshareCreateResponse.location
            };
        }

        public async Task<HttpResponseMessage> CreateArticle(FigshareMetaData metaData, string token)
        {
            string json = JsonConvert.SerializeObject(metaData,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            // Remove charset of content type to avoid 415 UNSUPPORTED MEDIA TYPE error.
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PostAsync(
                "https://api.figshare.com/v2/account/articles?access_token=" + token,
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> CreateFile(string name, int size, int articleId, string token)
        {
            string json = JsonConvert.SerializeObject(new FigshareFile() { name = name + ".zip", size = size },
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(
                $"https://api.figshare.com/v2/account/articles/{articleId}/files?access_token=" + token,
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> GetFileInfo(int articleId, int fileId, string token)
        {
            HttpResponseMessage response = await client.GetAsync(
                $"https://api.figshare.com/v2/account/articles/{articleId}/files/{fileId}?access_token=" + token
            );

            return response;
        }

        private async Task<HttpResponseMessage> UploadBytesToFile(byte[] file, string upload_url, string token)
        {
            ByteArrayContent content = new ByteArrayContent(file);

            HttpResponseMessage response = await client.PutAsync(
                upload_url + "/1?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content);

            return response;
        }

        private async Task<HttpResponseMessage> CompleteFileUpload(int articleId, int fileId, string token)
        {
            HttpResponseMessage response = await client.PostAsync(
                 $"https://api.figshare.com/v2/account/articles/{articleId}/files/{fileId}?access_token=" + token,
                 null);

            return response;
        }

        private async Task<HttpResponseMessage> PublishArticle(int articleId, string token)
        {
            HttpResponseMessage response = await client.PostAsync(
                 $"https://api.figshare.com/v2/account/articles/{articleId}/publish?access_token=" + token,
                 null);

            return response;
        }

        private async Task<HttpResponseMessage> DeleteArticle(int articleId, string token)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                 $"https://api.figshare.com/v2/account/articles/{articleId}?access_token=" + token);

            return response;
        }

        private async Task<PublishErrorResponse> PublishErrorResponse(HttpResponseMessage response)
        {
            return new PublishErrorResponse()
            {
                message = "An error occurred while publishing files to figshare. See figshare response for more detail.",
                publishingSystemResponse = await response.ToFigshareResponse()
            };
        }
    }
}
