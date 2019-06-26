using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class PublishingService : IPublishingService
    {
        private static HttpClient client = new HttpClient();

        public async Task<HttpResponseMessage> PublishObject(TestModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            HttpResponseMessage response = await client.PostAsync(
                "https://b2share.eudat.eu/api/records/?access_token=V1mifvQ2s7BI4QTcTVfWSyQZeCYC3GSyXjwiEaLzKPmlTUV1gOa9IMotR7NH",
                new StringContent(json, Encoding.UTF32, "application/json")
            );

            return response;
        }
    }
}
