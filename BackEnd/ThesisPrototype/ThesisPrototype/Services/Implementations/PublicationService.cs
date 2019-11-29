using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class PublicationService : IPublicationService
    {
        private readonly IMongoCollection<Publication> publications;
        private readonly IMongoCollection<PublicationBundle> publicationBundles;

        public PublicationService(IPublicationDatabaseSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);

            publications = database.GetCollection<Publication>(dbSettings.PublicationCollectionName);
            publicationBundles = database.GetCollection<PublicationBundle>(dbSettings.PublicationBundleCollectionName);
        }

        public Publication FindDuplicatePublication(string checksum)
        {
            return null;
        }

        public PublicationBundle FindDuplicatePublicationBundle(List<Publication> publications)
        {
            return null;
        }

        private bool IsDuplicateBundle(PublicationBundle bundle, List<Publication> publications)
        {
            return false;
        }

        public Publication CreatePublication(string sourceUrl, string publicationUrl, bool open_access, string checksum)
        {

            Publication publication = new Publication()
            {
                Checksum = checksum,
                Open_Access = open_access,
                PublicationUrl = publicationUrl,
                SourceUrl = sourceUrl
            };

            return publication;
        }

        public PublicationBundle CreatePublicationBundle(string[] publicationUrls, string[] publicationIds, string publicationUrl, bool open_access)
        {
            PublicationBundle publicationBundle = new PublicationBundle()
            {
                PublicationIds = publicationIds,
                PublicationUrls = publicationUrls,
                PublicationUrl = publicationUrl,
                Open_Access = open_access
            };

            return publicationBundle;
        }
    }
}
