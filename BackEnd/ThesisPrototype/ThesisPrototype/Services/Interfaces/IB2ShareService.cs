using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IB2ShareService : IPublishingService
    {
        Task<HttpResponseMessage> ListAllRecords();

        Task<HttpResponseMessage> DeleteDraftRecord(string recordId);
    }
}
