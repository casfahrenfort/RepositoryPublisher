using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Converters;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Dataverse;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class DataverseService : IDataverseService
    {
        private static HttpClient client = new HttpClient();

        public async Task<Response> PublishRepository(byte[] repositoryBytes, PublishInfo publishInfo)
        {
            DataverseMetaData dataverseMetaData = publishInfo.metaData.ToDataverseMetaData();
            HttpResponseMessage response = await CreateDataset(dataverseMetaData, publishInfo.token);
            
            if (!response.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(response);
            }

            DataverseCreateResponse dataverseCreateResponse = await response.ToDataverseCreateResponse();
            int datasetId = dataverseCreateResponse.data.id;

            response = await UploadFile(datasetId, repositoryBytes, publishInfo.repoName, publishInfo.token);
            
            if (!response.IsSuccessStatusCode)
            {
                await DeleteDataset(datasetId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            response = await PublishDataset(datasetId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                await DeleteDataset(datasetId, publishInfo.token);
                return await PublishErrorResponse(response);
            }
            
            dataverseCreateResponse = await response.ToDataverseCreateResponse();
            return new PublishResponse()
            {
                message = "Repository succesfully published.",
                publishUrl = dataverseCreateResponse.data.persistentUrl
            };
        }

        public async Task<Response> PublishMultipleRepositories(List<PublishInfo> publishInfos, List<Publication> duplicates, PublishInfo bundlePublishInfo)
        {
            List<PublishingSystemPublication> publications = new List<PublishingSystemPublication>();
            List<Response> responses = new List<Response>();

            for (int i = 0; i < publishInfos.Count; i++)
            {
                DataverseMetaData dataverseMetaData = publishInfos[i].metaData.ToDataverseMetaData();
                HttpResponseMessage response = await CreateDataset(dataverseMetaData, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDataset(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                DataverseCreateResponse createResponse = await response.ToDataverseCreateResponse();
                int datasetId = createResponse.data.id;

                response = await UploadFile(datasetId, publishInfos[i].snapshot.zippedBytes, publishInfos[i].repoName, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDataset(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                publications.Add(new PublishingSystemPublication()
                {
                    publicationId = datasetId.ToString(),
                    publishInfo = publishInfos[i]
                });
            }

            for (int i = 0; i < publications.Count; i++)
            {
                HttpResponseMessage response = await PublishDataset(int.Parse(publications[i].publicationId), publications[i].publishInfo.token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; j++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDataset(int.Parse(publications[j].publicationId), publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }
                else
                {
                    DataverseCreateResponse createResponse = await response.ToDataverseCreateResponse();
                    publications[i].publicationUrl = createResponse.data.persistentUrl;
                }
            }

            DataverseMetaData bundleDataverseMetaData = bundlePublishInfo.metaData.ToDataverseMetaData();
            //TODO: fix references to repositories
            /*bundleDataverseMetaData.references = publications.Select(x => x.publicationUrl)
                .Union(duplicates.Select(x => x.PublicationUrl))
                .ToArray();*/

            HttpResponseMessage bundleResponse = await CreateDataset(bundleDataverseMetaData, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(bundleResponse);
            }

            DataverseCreateResponse dataverseCreateResponse = await bundleResponse.ToDataverseCreateResponse();
            int bundleArticleId = dataverseCreateResponse.data.id;

            bundleResponse = await PublishDataset(bundleArticleId, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                await DeleteDataset(bundleArticleId, bundlePublishInfo.token);
                return await PublishErrorResponse(bundleResponse);
            }

            dataverseCreateResponse = await bundleResponse.ToDataverseCreateResponse();
            return await bundleResponse.ToMultiplePublishResponse(publications, dataverseCreateResponse.data.persistentUrl);
        }

        public async Task<HttpResponseMessage> CreateDataset(DataverseMetaData metaData, string token)
        {
            string json = JsonConvert.SerializeObject(metaData,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            // Remove charset of content type to avoid 415 UNSUPPORTED MEDIA TYPE error.
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PostAsync(
                "https://demo.dataverse.org/api/dataverses/harvard/datasets?key=" + token,
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> UploadFile(int datasetId, byte[] file, string fileName, string token)
        {
            ByteArrayContent content = new ByteArrayContent(file);

            MultipartFormDataContent formContent = new MultipartFormDataContent();
            formContent.Add(content, "file", fileName);

            HttpResponseMessage response = await client.PostAsync(
                $"https://demo.dataverse.org/api/datasets/{datasetId}/add?key={token}", 
                formContent);

            return response;
        }

        public async Task<HttpResponseMessage> PublishDataset(int datasetId, string token)
        {
            HttpResponseMessage response = await client.PostAsync(
                $"https://demo.dataverse.org/api/datasets/{datasetId}/actions/:publish?type=major&key={token}",
                null
            );

            return response;
        }

        public async Task<HttpResponseMessage> DeleteDataset(int datasetId, string token)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"https://demo.dataverse.org/api/datasets/{datasetId}?key={token}"
            );

            return response;
        }

        private async Task<PublishErrorResponse> PublishErrorResponse(HttpResponseMessage response)
        {
            return new PublishErrorResponse()
            {
                message = "An error occurred while publishing files to Harvard Dataverse. See Harvard Dataverse response for more detail.",
                publishingSystemResponse = await response.ToDataverseResponse()
            };
        }

    }
}
