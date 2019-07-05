using System.Net.Http;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IPublishingService
    {
        Task<HttpResponseMessage> PublishDraftRecord(string recordId);
    }
}
