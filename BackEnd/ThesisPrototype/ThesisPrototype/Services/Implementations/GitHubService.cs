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

        public Snapshot GetRepositorySnapshot(string gitHubUrl, string repoName, string snapshotId, string requestId)
        {
            Directory.CreateDirectory($"../Repos/{requestId}");
            Directory.CreateDirectory($"../Repos/{requestId}/Files");


            string repoPath = $"../Repos/{requestId}/{repoName}";
            string barePath = $"../Repos/{requestId}/Files";


            DirectoryHelper.SetAttributesNormal(new DirectoryInfo(barePath));
            FileStream fs = new FileStream(barePath + "/dummyRepo", FileMode.CreateNew);
            //fs.Seek(524288000, SeekOrigin.Begin);
            fs.Seek(1, SeekOrigin.Begin);
            fs.WriteByte(0);
            fs.Close();

            try
            {
                //CloneGitRepo(gitHubUrl, barePath);

                //Repository bareRepo = new Repository(barePath);
                string md5 = Guid.NewGuid().ToString();
                byte[] repoBytes;

                if(snapshotId == "none")
                {
                    repoBytes = compressionService.ZipBytes(barePath, repoName, $"../Repos/{requestId}");
                }
                else
                {
                    /*Commit commit = bareRepo.Commits.Where(x => x.Sha == snapshotId).First();

                    Repository.Clone(barePath, repoPath);
                    Repository repo = new Repository(repoPath);

                    CheckoutGitSnapshot(repo, commit);
                    md5 = CreateSnapshotChecksum(commit);

                    bareRepo.Dispose();
                    repo.Dispose();

                    DirectoryHelper.SetAttributesNormal(new DirectoryInfo(repoPath));
                    Directory.Delete($"{repoPath}/.git", true);*/

                    repoBytes = compressionService.ZipBytes(barePath, repoName, $"../Repos/{requestId}");
                }

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

                return repoCommits.ToRepoTree();
            }
            catch(Exception e)
            {
                DeleteRequestDirectory(guid.ToString());

                throw e;
            }
        }

        private void CloneGitRepo(string gitHubUrl, string repoPath)
        {
            Repository.Clone(gitHubUrl, repoPath, new CloneOptions() { IsBare = true });
        }

        private void CheckoutGitSnapshot(Repository repo, Commit commit)
        {
            repo.Checkout(commit.Tree, new string[] { "*" }, new CheckoutOptions() { CheckoutModifiers = CheckoutModifiers.Force });
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

        private string CreateSnapshotChecksum(Commit commit)
        {
            MD5 mD5 = MD5.Create();
            byte[] mD5bytes = mD5.ComputeHash(Encoding.ASCII.GetBytes(commit.Sha));

            string result = BitConverter.ToString(mD5bytes).Replace("-", string.Empty);

            mD5.Dispose();

            return result;
        }

        private string CreateRepoChecksum(Repository repo)
        {
            string shas = "";
            foreach(Commit commit in repo.Commits)
            {
                shas += commit.Sha;
            }

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
