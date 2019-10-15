using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models.Dataverse;

namespace ThesisPrototype.Converters
{
    public static class DataverseResponseConverter
    {
        public static async Task<DataverseResponse> ToDataverseResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new DataverseResponse()
            {
                message = (string)jsonResponse.message,
                status = (string)jsonResponse.status
            };
        }


        public static async Task<DataverseCreateResponse> ToDataverseCreateResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new DataverseCreateResponse()
            {
                status = (string)jsonResponse.status,
                data = new DataverseCreateResponseData()
                {
                    id = (int)jsonResponse.data.id,
                    persistentId = (string)jsonResponse.data.persistentId,
                    persistentUrl = (string)jsonResponse.data.persistentUrl
                }
            };
        }

    }
}
