using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IPublicationService
    {
        Publication FindDuplicatePublication(string checksum);

        void CreatePublication(string sourceUrl, string publicationUrl, bool open_access, string checksum);
    }
}
