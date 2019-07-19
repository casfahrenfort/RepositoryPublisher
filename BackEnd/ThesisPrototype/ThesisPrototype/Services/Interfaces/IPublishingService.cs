using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IPublishingService
    {
        Task<HttpResponseMessage> PublishRepository(byte[] repositoryBytes, string repositoryName, MetaData metaData);
    }
}
