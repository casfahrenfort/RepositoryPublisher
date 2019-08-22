using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Converters;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class B2ShareService : IB2ShareService
    {
        private static HttpClient client = new HttpClient();

        private readonly IConfiguration configuration;

        public B2ShareService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<Response> PublishMultipleRepositories(List<PublishInfo> publishInfos, List<Publication> duplicates, PublishInfo bundlePublishInfo)
        {
            List<PublishingSystemPublication> publications = new List<PublishingSystemPublication>();
            List<Response> responses = new List<Response>();

            for (int i = 0; i < publishInfos.Count; i++)
            {
                B2ShareMetaData b2ShareMetaData = publishInfos[i].metaData.ToB2ShareMetaData();
                HttpResponseMessage response = await CreateDraftRecord(b2ShareMetaData, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j <= i; i++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDraftRecord(publications[j].publicationId, publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                B2ShareDraftResponse draftResponse = await response.ToB2ShareDraftResponse();

                response = await UploadBytesToDraftRecord(publishInfos[i].snapshot.zippedBytes, $"{publishInfos[i].repoName}.zip", draftResponse.fileBucketId, publishInfos[i].token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j <= i; i++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDraftRecord(publications[j].publicationId, publishInfos[j].token);
                    }
                    return await PublishErrorResponse(response);
                }

                publications.Add(new PublishingSystemPublication()
                {
                    publicationId = draftResponse.recordId,
                    publishInfo = publishInfos[i]
                });
            }

            for (int i = 0; i < publications.Count; i++)
            {
                HttpResponseMessage response = await PublishDraftRecord(publications[i].publicationId, publications[i].publishInfo.token);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j <= i; i++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDraftRecord(publications[j].publicationId, publications[j].publishInfo.token);
                    }
                    return await PublishErrorResponse(response);
                }
                else
                {
                    B2SharePublishResponse publicationResponse = await response.ToB2SharePublishResponse();
                    publications[i].publicationUrl = publicationResponse.publicationUrl;
                }
            }

            B2ShareMetaData bundleB2ShareMetaData = bundlePublishInfo.metaData.ToB2ShareMetaData();
            bundleB2ShareMetaData.resource_types = publications
                .Select(p => new B2ShareResourceType() { resource_type = p.publicationUrl, resource_type_general = "Software" })
                .Union(duplicates.Select(d => new B2ShareResourceType() { resource_type = d.PublicationUrl, resource_type_general = "Software" }))
                .ToArray();

            HttpResponseMessage bundleResponse = await CreateDraftRecord(bundleB2ShareMetaData, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(bundleResponse);
            }

            B2ShareDraftResponse bundleDraftResponse = await bundleResponse.ToB2ShareDraftResponse();

            bundleResponse = await PublishDraftRecord(bundleDraftResponse.recordId, bundlePublishInfo.token);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                // Delete all drafts to ensure transactional nature of request
                await DeleteDraftRecord(bundleDraftResponse.recordId, bundlePublishInfo.token);
                return await PublishErrorResponse(bundleResponse);
            }

            return await bundleResponse.ToMultiplePublishResponse(publications);
        }

        public async Task<Response> PublishRepository(byte[] repoBytes, PublishInfo publishInfo)
        {
            B2ShareMetaData b2ShareMetaData = publishInfo.metaData.ToB2ShareMetaData();
            HttpResponseMessage response = await CreateDraftRecord(b2ShareMetaData, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                return await PublishErrorResponse(response);
            }

            B2ShareDraftResponse draftResponse = await response.ToB2ShareDraftResponse();

            response = await UploadBytesToDraftRecord(repoBytes, $"{publishInfo.repoName}.zip", draftResponse.fileBucketId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(draftResponse.recordId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            response = await PublishDraftRecord(draftResponse.recordId, publishInfo.token);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(draftResponse.recordId, publishInfo.token);
                return await PublishErrorResponse(response);
            }

            B2SharePublishResponse b2SharePublishResponse = await response.ToB2SharePublishResponse();
            return new PublishResponse()
            {
                message = "Repository succesfully published.",
                publishUrl = b2SharePublishResponse.publicationUrl
            };
        }

        private async Task<HttpResponseMessage> CreateDraftRecord(B2ShareMetaData metaData, string token)
        {
            metaData.community = "e9b9792e-79fb-4b07-b6b4-b9c2bd06d095";
            string json = JsonConvert.SerializeObject(metaData,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            // Remove charset of content type to avoid 415 UNSUPPORTED MEDIA TYPE error.
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PostAsync(
                "https://trng-b2share.eudat.eu/api/records/?access_token=" + token,
                content
            );

            return response;
        }

        private async Task<HttpResponseMessage> UploadBytesToDraftRecord(byte[] file, string fileName, string fileBucketId, string token)
        {
            ByteArrayContent content = new ByteArrayContent(file);

            HttpResponseMessage response = await client.PutAsync(
                "https://trng-b2share.eudat.eu/api/files/" + fileBucketId + "/" + fileName + "?access_token=" + token,
                content);

            return response;
        }

        private async Task<HttpResponseMessage> PublishDraftRecord(string recordId, string token)
        {
            string patchMessage = "[{\"op\": \"add\", \"path\":\"/community_specific\", \"value\": {}},{\"op\": \"add\", \"path\":\"/publication_state\", \"value\": \"submitted\"}]";
            StringContent content = new StringContent(patchMessage, Encoding.UTF8, "application/json-patch+json");
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PatchAsync(
                "https://trng-b2share.eudat.eu/api/records/" + recordId + "/draft?access_token=" + token,
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> ListAllRecords(string token)
        {
            HttpResponseMessage response = await client.GetAsync(
                "https://trng-b2share.eudat.eu/api/records/?drafts=1&size=50&access_token=" + token);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteDraftRecord(string recordId, string token)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                "https://trng-b2share.eudat.eu/api/records/" + recordId + "/draft?access_token=" + token
            );

            return response;
        }

        private async Task<PublishErrorResponse> PublishErrorResponse(HttpResponseMessage response)
        {
            return new PublishErrorResponse()
            {
                message = "An error occurred while publishing files to B2SHARE. See B2SHARE response for more detail.",
                publishingSystemResponse = await response.ToB2ShareResponse()
            };
        }

    }
}
