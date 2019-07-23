using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ThesisPrototype.Helpers;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class SubversionService : ISubversionService
    {
        private readonly ICompressionService compressionService;

        public SubversionService(ICompressionService compressionService)
        {
            this.compressionService = compressionService;
        }

        public Snapshot GetRepositorySnapshot(string svnUrl, string repoName)
        {
            string dumpPath = $"../Repos/{repoName}.svndump";
            string repoPath = $"../Repos/{repoName}";

            // Dump external SVN repo
            ShellHelper.Bash("svnrdump.exe", $"dump {svnUrl} -F {dumpPath}");
            // Create new empty local SVN repo
            ShellHelper.Bash("svnadmin.exe", $"create {repoPath}");
            // Load external dump to local repo
            ShellHelper.Bash("svnadmin.exe", $"load {repoPath} -F {dumpPath}");

            string md5 = CreateSvnChecksum(dumpPath);
            byte[] repoBytes = compressionService.ZipBytes(repoPath, repoName);

            File.Delete(dumpPath);
            DirectoryHelper.SetAttributesNormal(new DirectoryInfo(repoPath));
            Directory.Delete(repoPath, true);

            return new Snapshot()
            {
                checksum = md5,
                zippedBytes = repoBytes
            };
        }

        private string CreateSvnChecksum(string dumpPath)
        {
            MD5 mD5 = MD5.Create();
            byte[] mD5bytes = mD5.ComputeHash(File.ReadAllBytes(dumpPath));

            string result = BitConverter.ToString(mD5bytes).Replace("-", string.Empty);

            mD5.Dispose();

            return result;
        }
    }
}
