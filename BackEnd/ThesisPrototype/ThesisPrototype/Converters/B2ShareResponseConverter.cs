﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Converters
{
    public static class B2ShareResponseConverter
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
    }
}
