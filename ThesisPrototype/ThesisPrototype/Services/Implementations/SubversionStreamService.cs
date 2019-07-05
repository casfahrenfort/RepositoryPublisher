using SharpSvn;
using System.IO;
using System.Linq;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class SubversionStreamService : ISubversionStreamService
    {
        private readonly ICompressionService compressionService;

        public SubversionStreamService(ICompressionService compressionService)
        {
            this.compressionService = compressionService;
        }

        public Stream SvnStream(string svnUrl)
        {
            using (SvnClient client = new SvnClient())
            {
                //client.CheckOut(SvnUriTarget.FromString(svnUrl), "../svn");
            }

            return null;
        }
    }
}
