using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IPublishingService
    {
        Task<Response> PublishRepository(byte[] repositoryBytes, PublishInfo publishInfo);

        Task<Response> PublishMultipleRepositories(List<PublishInfo> publishInfos, List<Publication> duplicates, PublishInfo bundlePublishInfo);
    }
}
