using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.IO;
using System.IO.Compression;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class CompressionService : ICompressionService
    {
        /// <summary>
        /// Creates a GZipped Tar file from a source directory
        /// </summary>
        /// <param name="sourceDirectory">Input directory containing files to be added to GZipped tar archive</param>
        public MemoryStream CreateTarGzStream(string sourceDirectory)
        {
            //FileStream fs = new FileStream(outputTarFilename, FileMode.Create, FileAccess.Write, FileShare.None);
            MemoryStream ms = new MemoryStream();
            GZipStream gzipStream = new GZipStream(ms, CompressionLevel.Optimal);

            TarArchive tarArchive = TarArchive.CreateOutputTarArchive(gzipStream);
            AddDirectoryFilesToTar(tarArchive, sourceDirectory, true);

            ms.Position = 0;
            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            MemoryStream outStream = new MemoryStream(compressed);
            return outStream;
        }

        /// <summary>
        /// Recursively adds folders and files to archive
        /// </summary>
        /// <param name="tarArchive"></param>
        /// <param name="sourceDirectory"></param>
        /// <param name="recurse"></param>
        private TarArchive AddDirectoryFilesToTar(TarArchive tarArchive, string sourceDirectory, bool recurse)
        {
            // Recursively add sub-folders
            if (recurse)
            {
                string[] directories = Directory.GetDirectories(sourceDirectory);
                foreach (string directory in directories)
                    AddDirectoryFilesToTar(tarArchive, directory, recurse);
            }

            // Add files
            string[] filenames = Directory.GetFiles(sourceDirectory);
            foreach (string filename in filenames)
            {
                TarEntry tarEntry = TarEntry.CreateEntryFromFile(filename);
                // Remove "../" from filenames because this will cause errors when unpacking
                tarEntry.Name = filename.Remove(0, 3);
                tarArchive.WriteEntry(tarEntry, true);
            }

            return tarArchive;
        }

    }
}
