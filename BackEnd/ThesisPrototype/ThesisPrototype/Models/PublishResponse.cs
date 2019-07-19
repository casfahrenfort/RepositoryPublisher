using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models
{
    public class PublishResponse
    {
        public string message;
        public string publishUrl;
    }

    public class PublishErrorResponse
    {
        public string message;
        public B2ShareResponse b2ShareResponse;
    }
}
