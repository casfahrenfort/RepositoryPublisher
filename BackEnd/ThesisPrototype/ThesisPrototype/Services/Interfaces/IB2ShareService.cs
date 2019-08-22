using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IB2ShareService : IPublishingService
    {
        Task<HttpResponseMessage> ListAllRecords(string token);

        Task<HttpResponseMessage> DeleteDraftRecord(string recordId, string token);
    }
}
