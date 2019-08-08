using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ThesisPrototype.Converters;
using ThesisPrototype.Helpers;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Repo;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Services.Implementations
{
    public class GitHubService : IGitHubService
    {
        private readonly ICompressionService compressionService;

        public GitHubService(ICompressionService compressionService)
        {
            this.compressionService = compressionService;
        }

        public Snapshot GetRepositorySnapshot(string gitHubUrl, string repoName, string requestId)
        {
            Directory.CreateDirectory($"../Repos/{requestId}");
            string repoPath = $"../Repos/{requestId}/{repoName}";

            try
            {
                CloneGitRepo(gitHubUrl, repoPath);

                DirectoryHelper.SetAttributesNormal(new DirectoryInfo(repoPath));

                string md5 = CreateGitChecksum(repoPath);

                byte[] repoBytes = compressionService.ZipBytes(repoPath, repoName, $"../Repos/{requestId}");

                DeleteRequestDirectory(requestId);

                return new Snapshot()
                {
                    checksum = md5,
                    zippedBytes = repoBytes
                };
            }
            catch (Exception e)
            {
                DeleteRequestDirectory(requestId);

                throw e;
            }
        }

        public RepoTree GetRepositoryTree(string url)
        {
            // Randomize folder name to avoid collisions
            string guid = Guid.NewGuid().ToString().Split('-').First();
            Directory.CreateDirectory($"../Repos/{guid}");
            string repoPath = $"../Repos/{guid}/repo";

            try
            {
                CloneGitRepo(url, repoPath);

                Repository repo = new Repository(repoPath);

                RepoCommit[] repoCommits = repo.Commits.Select(c => c.ToRepoCommit()).ToArray();

                repo.Dispose();

                DeleteRequestDirectory(guid.ToString());

                return new RepoTree()
                {
                    commits = repoCommits
                };
            }
            catch(Exception e)
            {
                DeleteRequestDirectory(guid);

                throw e;
            }
        }

        private void CloneGitRepo(string gitHubUrl, string repoPath)
        {
            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(gitHubUrl, repoPath);
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

        private string CreateGitChecksum(string repoPath)
        {
            string shas = "";

            Repository repo = new Repository(repoPath);
            foreach (Commit commit in repo.Commits)
            {
                shas += commit.Sha;
            }

            repo.Dispose();

            MD5 mD5 = MD5.Create();
            byte[] mD5bytes = mD5.ComputeHash(Encoding.ASCII.GetBytes(shas));

            string result = BitConverter.ToString(mD5bytes).Replace("-", string.Empty);

            mD5.Dispose();

            return result;
        }

        private void WriteTreeEntry(TreeEntry treeEntry, string commitPath)
        {
            string path = commitPath + "/" + treeEntry.Path;
            if (treeEntry.TargetType == TreeEntryTargetType.Blob)
            {
                Blob blob = (Blob)treeEntry.Target;
                File.WriteAllText(path, blob.GetContentText());
            }
            else if (treeEntry.TargetType == TreeEntryTargetType.Tree)
            {
                if (treeEntry.Mode == Mode.Directory)
                {
                    Directory.CreateDirectory(path);
                }

                foreach (TreeEntry entry in (Tree)treeEntry.Target)
                {
                    WriteTreeEntry(entry, commitPath);
                }
            }
        }
    }
}
