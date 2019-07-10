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

        Task<HttpResponseMessage> UploadStreamToDraftRecord(byte[] file, string fileName, string fileBucketId);

        Task<HttpResponseMessage> ListAllRecords();
    }
}
