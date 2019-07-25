using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Interfaces
{
    public interface ICompressionService
    {
        byte[] ZipBytes(string sourceDirectory, string archiveName, string destPath);
    }
}
