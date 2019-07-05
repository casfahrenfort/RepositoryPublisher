using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Interfaces
{
    public interface ICompressionService
    {
        MemoryStream CreateTarGzStream(string sourceDirectory);
    }
}
