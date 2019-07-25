using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Models.B2Share;

namespace ThesisPrototype.Converters
{
    public static class ResponseConverter
    {
        public static async Task<B2ShareResponse> ToB2ShareResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new B2ShareResponse()
            {
                message = (string)jsonResponse.message,
                status = (string)jsonResponse.status
            };
        }

        public static async Task<B2ShareDraftResponse> ToB2ShareDraftResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string recordId = jsonResponse.id;
            string fileBucketId = jsonResponse.links.files;
            fileBucketId = fileBucketId.Split('/').Last();

            return new B2ShareDraftResponse()
            {
                recordId = recordId,
                fileBucketId = fileBucketId
            };
        }

        public static async Task<B2SharePublishResponse> ToB2SharePublishResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string publicationUrl = (string)jsonResponse.links.publication;

            return new B2SharePublishResponse()
            {
                publicationUrl = publicationUrl
            };
        }

        public static async Task<B2ShareMultiplePublishResponse> ToB2ShareMultiplePublishResponse(this HttpResponseMessage httpResponseMessage, List<B2SharePublication> publicationInfos)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string publicationUrl = (string)jsonResponse.links.publication;

            return new B2ShareMultiplePublishResponse()
            {
                bundlePublicationUrl = publicationUrl,
                bundlePublicationInfos = publicationInfos
            };
        }

    }
}
