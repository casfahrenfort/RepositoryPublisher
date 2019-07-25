using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Converters;
using ThesisPrototype.Models;
using ThesisPrototype.Models.B2Share;
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

        public async Task<Response> PublishMultipleRepositories(List<PublishInfo> publishInfos, PublishInfo bundlePublishInfo)
        {
            List<B2SharePublication> publications = new List<B2SharePublication>();
            List<Response> responses = new List<Response>();

            for (int i = 0; i < publishInfos.Count; i++)
            {
                B2ShareMetaData b2ShareMetaData = publishInfos[i].metaData.ToB2ShareMetaData();
                HttpResponseMessage response = await CreateDraftRecord(b2ShareMetaData);

                if (!response.IsSuccessStatusCode)
                {
                    return await response.ToB2ShareResponse();
                }

                B2ShareDraftResponse draftResponse = await response.ToB2ShareDraftResponse();

                response = await UploadBytesToDraftRecord(publishInfos[i].snapshot.zippedBytes, $"{publishInfos[i].repoName}.zip", draftResponse.fileBucketId);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; i++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDraftRecord(publications[j].recordId);
                    }
                    return await response.ToB2ShareResponse();
                }

                publications.Add(new B2SharePublication()
                {
                    recordId = draftResponse.recordId,
                    publishInfo = publishInfos[i]
                });
            }

            for (int i = 0; i < publications.Count; i++)
            {
                HttpResponseMessage response = await PublishDraftRecord(publications[i].recordId);

                if (!response.IsSuccessStatusCode)
                {
                    for (int j = 0; j < i; i++)
                    {
                        // Delete all drafts to ensure transactional nature of request
                        await DeleteDraftRecord(publications[i].recordId);
                    }
                    return await response.ToB2ShareResponse();
                }
                else
                {
                    B2SharePublishResponse publicationResponse = await response.ToB2SharePublishResponse();
                    publications[i].publicationUrl = publicationResponse.publicationUrl;
                }
            }

            B2ShareMetaData bundleB2ShareMetaData = bundlePublishInfo.metaData.ToB2ShareMetaData();
            bundleB2ShareMetaData.resource_types = publications.Select(p => new B2ShareResourceType() { resource_type = p.publicationUrl, resource_type_general = "Software" }).ToArray();
            HttpResponseMessage bundleResponse = await CreateDraftRecord(bundleB2ShareMetaData);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                return await bundleResponse.ToB2ShareResponse();
            }

            B2ShareDraftResponse bundleDraftResponse = await bundleResponse.ToB2ShareDraftResponse();

            bundleResponse = await PublishDraftRecord(bundleDraftResponse.recordId);

            if (!bundleResponse.IsSuccessStatusCode)
            {
                // Delete all drafts to ensure transactional nature of request
                await DeleteDraftRecord(bundleDraftResponse.recordId);
                return await bundleResponse.ToB2ShareResponse();
            }

            return await bundleResponse.ToB2ShareMultiplePublishResponse(publications);
        }

        public async Task<Response> PublishRepository(byte[] repoBytes, string repoName, MetaData metaData)
        {
            B2ShareMetaData b2ShareMetaData = metaData.ToB2ShareMetaData();
            HttpResponseMessage response = await CreateDraftRecord(b2ShareMetaData);

            if (!response.IsSuccessStatusCode)
            {
                return await response.ToB2ShareResponse();
            }

            B2ShareDraftResponse draftResponse = await response.ToB2ShareDraftResponse();

            response = await UploadBytesToDraftRecord(repoBytes, $"{repoName}.zip", draftResponse.fileBucketId);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(draftResponse.recordId);
                return await response.ToB2ShareResponse();
            }

            response = await PublishDraftRecord(draftResponse.recordId);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(draftResponse.recordId);
                return await response.ToB2ShareResponse();
            }

            return await response.ToB2SharePublishResponse();
        }

        private async Task<HttpResponseMessage> CreateDraftRecord(B2ShareMetaData metaData)
        {
            metaData.community = "e9b9792e-79fb-4b07-b6b4-b9c2bd06d095";
            string json = JsonConvert.SerializeObject(metaData,
                Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            // Remove charset of content type to avoid 415 UNSUPPORTED MEDIA TYPE error.
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PostAsync(
                "https://trng-b2share.eudat.eu/api/records/?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content
            );

            return response;
        }

        private async Task<HttpResponseMessage> UploadBytesToDraftRecord(byte[] file, string fileName, string fileBucketId)
        {
            ByteArrayContent content = new ByteArrayContent(file);

            HttpResponseMessage response = await client.PutAsync(
                "https://trng-b2share.eudat.eu/api/files/" + fileBucketId + "/" + fileName + "?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content);

            return response;
        }

        private async Task<HttpResponseMessage> PublishDraftRecord(string recordId)
        {
            string patchMessage = "[{\"op\": \"add\", \"path\":\"/community_specific\", \"value\": {}},{\"op\": \"add\", \"path\":\"/publication_state\", \"value\": \"submitted\"}]";
            StringContent content = new StringContent(patchMessage, Encoding.UTF8, "application/json-patch+json");
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PatchAsync(
                "https://trng-b2share.eudat.eu/api/records/" + recordId + "/draft?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> ListAllRecords()
        {
            HttpResponseMessage response = await client.GetAsync(
                "https://trng-b2share.eudat.eu/api/records/?drafts=1&size=50&access_token=" + configuration["B2SHAREtrngAccessToken"]);

            return response;
        }

        public async Task<HttpResponseMessage> DeleteDraftRecord(string recordId)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                "https://trng-b2share.eudat.eu/api/records/" + recordId + "/draft?access_token=" + configuration["B2SHAREtrngAccessToken"]
            );

            return response;
        }

    }
}
