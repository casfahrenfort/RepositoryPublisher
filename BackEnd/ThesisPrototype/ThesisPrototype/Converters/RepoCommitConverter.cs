using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ThesisPrototype.Models.Repo;

namespace ThesisPrototype.Converters
{
    public static class RepoCommitConverter
    {
        public static RepoCommit ToRepoCommit(this Commit commit)
        {
            List<RepoDirectory> directories = new List<RepoDirectory>();
            List<string> files = new List<string>();
            
            foreach (TreeEntry t in commit.Tree)
            {
                if (t.TargetType == TreeEntryTargetType.Tree)
                {
                    directories.Add(t.ToRepoDirectory());
                }
                else if (t.TargetType == TreeEntryTargetType.Blob)
                {
                    files.Add(t.Name);
                }
            }

            return new RepoCommit()
            {
                date = commit.Author.When.Date,
                commitId = $"{commit.Sha.Substring(0, 6)} {commit.Message}",
                files = files.ToArray(),
                directories = directories.ToArray()
            };
        }

        private static RepoDirectory ToRepoDirectory(this TreeEntry entry)
        {
            List<RepoDirectory> directories = new List<RepoDirectory>();
            List<string> files = new List<string>();

            Tree tree = (Tree)entry.Target;

            foreach (TreeEntry t in tree)
            {
                if (t.TargetType == TreeEntryTargetType.Tree)
                {
                    directories.Add(t.ToRepoDirectory());
                }
                else if (t.TargetType == TreeEntryTargetType.Blob)
                {
                    files.Add(t.Name);
                }
            }

            return new RepoDirectory(entry.Name)
            {
                directories = directories.ToArray(),
                files = files.ToArray()
            };
        }

        public static RepoTree ToRepoTree(this string svnLogs, string svnUrl)
        {
            string[] splitLogs = svnLogs.Split("\r\n");

            List<RepoCommit> commits = new List<RepoCommit>();
            
            for (int i = 0; i < splitLogs.Length - 2; i+= 4) // The last two lines can be disregarded
            {
                int revNumber = (int)Char.GetNumericValue(splitLogs[i + 1][1]);
                string message = $"r{revNumber} {splitLogs[i + 2]}";
                DateTime date = DateTime.ParseExact(splitLogs[i + 1].Split('|')[2].Split('(')[0], " yyyy-MM-dd HH:mm:ss K ", System.Globalization.CultureInfo.InvariantCulture);
                
                string svnList = ShellHelper.Bash("svn.exe", $"list {svnUrl} -R -r {revNumber}");

                commits.Add(svnList.ToRepoCommit(message, date));
            }

            return new RepoTree()
            {
                commits = commits.ToArray()
            };
        }

        private static RepoCommit ToRepoCommit(this string svnList, string message, DateTime date)
        {
            string[] splitList = svnList.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, RepoDirectory> rootDirectories = new Dictionary<string, RepoDirectory>();
            List<string> files = new List<string>();

            RepoDirectory root = new RepoDirectory("root");

            for (int i = 0; i < splitList.Length; i++)
            {
                RepoDirectory current = root;

                bool isFolder = splitList[i].Last() == '/';

                string[] line = splitList[i].Split('/', StringSplitOptions.RemoveEmptyEntries);

                for(int j = 0; j < line.Length; j++)
                {
                    if (j == line.Length - 1)
                    {
                        if (isFolder)
                        {
                            current.AddDirectory(line[j]);
                        }
                        else
                        {
                            current.AddFile(line[j]);
                        }
                    }
                    else
                    {
                        current = current.GetDirectory(line[j]);
                    }
                }
            }

            return new RepoCommit()
            {
                directories = root.directories,
                files = root.files,
                date = date,
                commitId = message
            };
        }
    }
}
