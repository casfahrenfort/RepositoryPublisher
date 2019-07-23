using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ThesisPrototype.Helpers;
using ThesisPrototype.Models;
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

        public Snapshot GetRepositorySnapshot(string gitHubUrl, string repoName)
        {
            string repoPath = CloneGitRepo(gitHubUrl, repoName);

            DirectoryHelper.SetAttributesNormal(new DirectoryInfo(repoPath));

            string md5 = CreateGitChecksum(repoPath);

            byte[] repoBytes = compressionService.ZipBytes(repoPath, repoName);

            DeleteGitRepo(repoPath);

            return new Snapshot()
            {
                checksum = md5,
                zippedBytes = repoBytes
            };
        }

        private List<DraftFile> GitFolderAndCommitStreams(string gitHubUrl)
        {
            string repoPath = CloneGitRepo(gitHubUrl, "");

            List<DraftFile> result = new List<DraftFile>();

            result.Add(GitFolderDraftFile(repoPath));

            Repository repo = new Repository(repoPath);
            foreach (Commit commit in repo.Commits)
            {
                result.Add(GitCommitDraftFile(commit));
            }

            repo.Dispose();

            DeleteGitRepo(repoPath);

            return result;
        }

        private DraftFile GitFolderDraftFile(string repoPath)
        {
            byte[] gitBites = compressionService.ZipBytes(repoPath + "/.git", ".git");

            return new DraftFile()
            {
                name = ".git.zip",
                bytes = gitBites
            };
        }

        private DraftFile GitCommitDraftFile(Commit commit)
        {
            string commitPath = "../" + commit.Sha.Substring(0, 8) + " " + commit.MessageShort;
            DirectoryInfo commitDirectory = Directory.CreateDirectory(commitPath);

            foreach (TreeEntry treeEntry in commit.Tree)
            {
                WriteTreeEntry(treeEntry, commitPath);
            }

            byte[] commitBytes = compressionService.ZipBytes(commitPath, commit.Sha.Substring(0, 8) + " " + commit.MessageShort);
            Directory.Delete(commitPath, true);

            return new DraftFile()
            {
                name = commit.Sha.Substring(0, 8) + " " + commit.MessageShort + ".zip",
                bytes = commitBytes
            };

        }

        private string CloneGitRepo(string gitHubUrl, string repoName)
        {
            string repoPath = "../Repos/" + repoName;

            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(gitHubUrl, repoPath);
            }

            return repoPath;
        }

        private void DeleteGitRepo(string repoPath)
        {
            Directory.Delete(repoPath, true);
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
