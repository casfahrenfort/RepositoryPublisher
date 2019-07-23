using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class PublicationService : IPublicationService
    {
        private readonly IMongoCollection<Publication> publications;

        public PublicationService(IPublicationDatabaseSettings dbSettings)
        {
            var client = new MongoClient(dbSettings.ConnectionString);
            var database = client.GetDatabase(dbSettings.DatabaseName);

            publications = database.GetCollection<Publication>(dbSettings.PublicationCollectionName);
        }

        public Publication FindDuplicatePublication(string checksum)
        {
            List<Publication> pubs = publications.Find<Publication>(p => p.Checksum == checksum).ToList();
            
            if (pubs.Count == 0)
            {
                return null;
            }
            else
            {
                foreach(Publication p in pubs)
                {
                    if (p.Open_Access)
                    {
                        return p;
                    }
                }
            }

            return null;
        }

        public void CreatePublication(string sourceUrl, string publicationUrl, bool open_access, string checksum)
        {
            Publication publication = new Publication()
            {
                Checksum = checksum,
                Open_Access = open_access,
                PublicationUrl = publicationUrl,
                SourceUrl = sourceUrl
            };

            publications.InsertOne(publication);
        }
    }
}
