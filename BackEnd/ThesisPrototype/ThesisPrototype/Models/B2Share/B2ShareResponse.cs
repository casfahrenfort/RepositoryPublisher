using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThesisPrototype.Models.B2Share;

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

    public class B2ShareMultiplePublishResponse : Response
    {
        public string bundlePublicationUrl;
        public List<B2SharePublication> bundlePublicationInfos;
    }
}
