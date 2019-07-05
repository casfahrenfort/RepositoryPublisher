using LibGit2Sharp;
using System.IO;
using System.Linq;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class GitHubStreamService : IGitHubStreamService
    {
        private readonly ICompressionService compressionService;

        public GitHubStreamService(ICompressionService compressionService)
        {
            this.compressionService = compressionService;
        }

        public Stream GitFolderStream(string gitHubUrl)
        {
            string repoName = gitHubUrl.Split('/').Last();
            string repoPath = "../" + repoName;

            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(gitHubUrl, repoPath);
            }
            
            Stream resultStream = compressionService.CreateTarGzStream(repoPath + "/.git");

            //Directory.Delete(repoPath, true);

            return resultStream;
        }

    }
}
