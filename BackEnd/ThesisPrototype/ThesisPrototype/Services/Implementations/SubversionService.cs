using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ThesisPrototype.Converters;
using ThesisPrototype.Helpers;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Repo;
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

        public Snapshot GetRepositorySnapshot(string svnUrl, string repoName, string requestId)
        {
            Directory.CreateDirectory($"../Repos/{requestId}");
            string repoPath = $"../Repos/{requestId}/{repoName}";

            try
            {
                string dumpPath = $"../Repos/{requestId}/{repoName}.svndump";

                // Dump external SVN repo
                ShellHelper.Bash("svnrdump.exe", $"dump {svnUrl} -F {dumpPath}");
                // Create new empty local SVN repo
                ShellHelper.Bash("svnadmin.exe", $"create {repoPath}");
                // Load external dump to local repo
                ShellHelper.Bash("svnadmin.exe", $"load {repoPath} -F {dumpPath}");

                string md5 = CreateSvnChecksum(dumpPath);
                byte[] repoBytes = compressionService.ZipBytes(repoPath, repoName, $"../Repos/{requestId}");

                File.Delete(dumpPath);
                DeleteRequestDirectory(requestId);

                return new Snapshot()
                {
                    checksum = md5,
                    zippedBytes = repoBytes
                };
            }
            catch(Exception e)
            {
                DeleteRequestDirectory(requestId);

                throw e;
            }
        }
        
        public RepoTree GetRepositoryTree(string url)
        {
            try
            {
                string logs = ShellHelper.Bash("svn.exe", $"log {url}");

                return logs.ToRepoTree(url);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void DeleteRequestDirectory(string requestId)
        {
            string path = $"../Repos/{requestId}";
            DirectoryHelper.SetAttributesNormal(new DirectoryInfo(path));
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
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
