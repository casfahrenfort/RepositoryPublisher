using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models.Figshare;

namespace ThesisPrototype.Converters
{
    public static class FigshareResponseConverter
    {
        public static async Task<FigshareResponse> ToFigshareResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new FigshareResponse()
            {
                message = (string)jsonResponse.message,
                code = (string)jsonResponse.code
            };
        }

        public static async Task<FigshareCreateResponse> ToFigshareCreateResponse(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new FigshareCreateResponse()
            {
                location = (string)jsonResponse.location
            };
        }

        public static async Task<FigshareFile> ToFigshareFile(this HttpResponseMessage httpResponseMessage)
        {
            string jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            return new FigshareFile()
            {
                name = (string)jsonResponse.name,
                size = (int)jsonResponse.size,
                id = (int)jsonResponse.id,
                computed_md5 = (string)jsonResponse.computed_md5,
                download_url = (string)jsonResponse.download_url,
                is_link_only = (bool)jsonResponse.is_link_only,
                preview_state = (string)jsonResponse.preview_state,
                status = (string)jsonResponse.status,
                supplied_md5 = (string)jsonResponse.supplied_md5,
                upload_token = (string)jsonResponse.upload_token,
                upload_url = (string)jsonResponse.upload_url,
                viewer_type = (string)jsonResponse.viewer_type

            };
        }
    }
}
