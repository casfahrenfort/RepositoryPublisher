using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Figshare
{
    public class FigshareResponse : Response
    {
        public string message;
        public string code;
    }

    public class FigshareCreateResponse : Response
    {
        public string location;
    }
}
