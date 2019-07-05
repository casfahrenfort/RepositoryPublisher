using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IDraftingService
    {
        Task<HttpResponseMessage> CreateDraftRecord(MetaData metadata);

        Task<HttpResponseMessage> DeleteDraftRecord(string recordId);

        Task<HttpResponseMessage> UploadStreamToDraftRecord(Stream stream, string fileName, string fileBucketId);
    }
}
