using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class DraftingService : IDraftingService
    {
        private static HttpClient client = new HttpClient();

        private readonly IConfiguration configuration;

        public DraftingService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<HttpResponseMessage> CreateDraftRecord(MetaData metaData)
        {
            metaData.open_access = false;
            metaData.community = "e9b9792e-79fb-4b07-b6b4-b9c2bd06d095";
            string json = JsonConvert.SerializeObject(metaData);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            // Remove charset of content type to avoid 415 UNSUPPORTED MEDIA TYPE error.
            content.Headers.ContentType.CharSet = "";

            HttpResponseMessage response = await client.PostAsync(
                "https://trng-b2share.eudat.eu/api/records/?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content
            );

            return response;
        }

        public async Task<HttpResponseMessage> DeleteDraftRecord(string recordId)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                "https://trng-b2share.eudat.eu/api/records/" + recordId + "/draft?access_token=" + configuration["B2SHAREtrngAccessToken"]
            );

            return response;
        }

        public async Task<HttpResponseMessage> UploadStreamToDraftRecord(byte[] file, string fileName, string fileBucketId)
        {
            ByteArrayContent content = new ByteArrayContent(file);
            //StreamContent streamContent = new StreamContent(stream);
            //streamContent.Headers.Add("Content-Encoding", "gzip");
            //streamContent.Headers.ContentLength = stream.Length;

            HttpResponseMessage response = await client.PutAsync(
                "https://trng-b2share.eudat.eu/api/files/" + fileBucketId + "/" + fileName + "?access_token=" + configuration["B2SHAREtrngAccessToken"],
                content);

            return response;
        }

        public async Task<HttpResponseMessage> ListAllRecords()
        {
            HttpResponseMessage response = await client.GetAsync(
                "https://trng-b2share.eudat.eu/api/records/?drafts=1&size=50&access_token=" + configuration["B2SHAREtrngAccessToken"]);

            return response;
        }

    }
}
