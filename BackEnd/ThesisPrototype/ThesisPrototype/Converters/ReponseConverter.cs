using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Converters
{
    public static class ReponseConverter
    {
        public static async Task<MultiplePublishResponse> ToMultiplePublishResponse(this HttpResponseMessage httpResponseMessage, List<PublishingSystemPublication> publicationInfos, string publicationUrl = "")
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            if (publicationUrl == "")
            {
                publicationUrl = (string)jsonResponse.links.publication;
            }

            return new MultiplePublishResponse()
            {
                bundlePublicationUrl = publicationUrl,
                bundlePublicationInfos = publicationInfos
            };
        }
    }
}
