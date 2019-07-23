using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ThesisPrototype.Models
{
    public class Publication
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("SourceURL")]
        public string SourceUrl { get; set; }

        [BsonElement("PublicationURL")]
        public string PublicationUrl { get; set; }

        [BsonElement("Checksum")]
        public string Checksum { get; set; }

        [BsonElement("OpenAccess")]
        public bool Open_Access { get; set; }
    }
}
