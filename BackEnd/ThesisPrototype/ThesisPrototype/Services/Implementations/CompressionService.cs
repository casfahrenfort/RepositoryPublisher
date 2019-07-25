using System.IO;
using System.IO.Compression;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class CompressionService : ICompressionService
    {
        public byte[] ZipBytes(string sourceDirectory, string archiveName, string destPath)
        {
            string zipPath = $"{destPath}/{archiveName}.zip";

            ZipFile.CreateFromDirectory(sourceDirectory, zipPath);
            
            byte[] bytes = File.ReadAllBytes(zipPath);

            File.Delete(zipPath);

            return bytes;
        }
    }
}
