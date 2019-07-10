using LibGit2Sharp;
using System.IO;
using System.Linq;
using ThesisPrototype.Services.Interfaces;
using ICSharpCode.SharpZipLib;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Collections.Generic;
using System;
using ThesisPrototype.Models;
using System.Threading.Tasks;

namespace ThesisPrototype.Services.Implementations
{
    public class GitHubService : IGitHubService
    {
        private readonly ICompressionService compressionService;

        public GitHubService(ICompressionService compressionService)
        {
            this.compressionService = compressionService;
        }

        public List<DraftFile> GitFolderAndCommitStreams(string gitHubUrl)
        {
            string repoPath = CloneGitRepo(gitHubUrl);

            List<DraftFile> result = new List<DraftFile>();

            result.Add(GitFolderDraftFile(repoPath));

            Repository repo = new Repository(repoPath);
            foreach (Commit commit in repo.Commits)
            {
                result.Add(GitCommitDraftFile(commit));
            }

            repo.Dispose();

            SetAttributesNormal(new DirectoryInfo(repoPath));
            Directory.Delete(repoPath, true);

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

        private string CloneGitRepo(string gitHubUrl)
        {
            string repoName = gitHubUrl.Split('/').Last();
            string repoPath = "../" + repoName;

            if (!Directory.Exists(repoPath))
            {
                Repository.Clone(gitHubUrl, repoPath);
            }

            return repoPath;
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
                if(treeEntry.Mode == Mode.Directory)
                {
                    Directory.CreateDirectory(path);
                }

                foreach(TreeEntry entry in (Tree)treeEntry.Target)
                {
                    WriteTreeEntry(entry, commitPath);
                }
            }
        }

        private void SetAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                SetAttributesNormal(subDir);
                subDir.Attributes = FileAttributes.Normal;
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
                file.IsReadOnly = false;
            }
        }

    }
}
