using System.Collections.Generic;

namespace ThesisPrototype.Models
{
    public class B2ShareResponse : Response
    {
        public string status;
        public string message;
    }

    public class B2ShareDraftResponse : Response
    {
        public string recordId;
        public string fileBucketId;
    }

    public class B2SharePublishResponse : Response
    {
        public string publicationUrl;
    }
}
