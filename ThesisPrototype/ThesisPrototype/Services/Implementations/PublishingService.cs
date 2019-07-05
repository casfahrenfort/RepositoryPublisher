using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class PublishingService : IPublishingService
    {
        private static HttpClient client = new HttpClient();

        private readonly IConfiguration configuration;

        public PublishingService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<HttpResponseMessage> PublishDraftRecord(string recordId)
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
    }
}
