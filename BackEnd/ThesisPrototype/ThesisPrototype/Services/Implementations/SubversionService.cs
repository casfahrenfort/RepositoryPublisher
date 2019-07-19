using System.IO;
using System.Linq;
using ThesisPrototype.Helpers;
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

        public byte[] GetRepositorySnapshot(string svnUrl, string repoName)
        {
            string dumpPath = $"../Repos/{repoName}.svndump";
            string repoPath = $"../Repos/{repoName}";

            // Dump external SVN repo
            ShellHelper.Bash("svnrdump.exe", $"dump {svnUrl} -F {dumpPath}");
            // Create new empty local SVN repo
            ShellHelper.Bash("svnadmin.exe", $"create {repoPath}");
            // Load external dump to local repo
            ShellHelper.Bash("svnadmin.exe", $"load {repoPath} -F {dumpPath}");

            byte[] repoBytes = compressionService.ZipBytes(repoPath, repoName);

            File.Delete(dumpPath);
            DirectoryHelper.SetAttributesNormal(new DirectoryInfo(repoPath));
            Directory.Delete(repoPath, true);

            return repoBytes;
        }
    }
}
