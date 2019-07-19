using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
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

        public async Task<HttpResponseMessage> PublishRepository(byte[] repoBytes, string repoName, MetaData metaData)
        {
            B2ShareMetaData b2ShareMetaData = metaData.ToB2ShareMetaData();
            HttpResponseMessage response = await CreateDraftRecord(b2ShareMetaData);

            if (!response.IsSuccessStatusCode)
            {
                return response;
            }

            string jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string recordId = jsonResponse.id;
            string fileBucketId = jsonResponse.links.files;
            fileBucketId = fileBucketId.Split('/').Last();

            response = await UploadBytesToDraftRecord(repoBytes, $"{repoName}.zip", fileBucketId);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(recordId);

                jsonString = await response.Content.ReadAsStringAsync();
                jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
                return response;
            }

            response = await PublishDraftRecord(recordId);

            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await DeleteDraftRecord(recordId);
                return response;
            }

            return response;
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
