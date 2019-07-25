using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models
{
    public class PublicationBundle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("OpenAccess")]
        public bool Open_Access { get; set; }

        [BsonElement("PublicationURL")]
        public string PublicationUrl { get; set; }

        [BsonElement("PublicationURLs")]
        public string[] PublicationUrls { get; set; }

        [BsonElement("PublicationIDs")]
        public string[] PublicationIds { get; set; }

        public PublicationBundle()
        {
            PublicationUrls = new string[0];
            PublicationIds = new string[0];
        }
    }
}
