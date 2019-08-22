using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models
{
    public class PublishResponse : Response
    {
        public string message;
        public string publishUrl;
    }

    public class PublishErrorResponse : Response
    {
        public string message;
        public Response publishingSystemResponse;
    }

    public class MultiplePublishResponse : Response
    {
        public string message;
        public string bundlePublicationUrl;
        public List<PublishingSystemPublication> bundlePublicationInfos;
    }
}
