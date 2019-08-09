using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Repo
{
    public class RepoTree
    {
        public RepoWeek[] repoWeeks;

        public RepoTree()
        {
            repoWeeks = new RepoWeek[0];
        }

        public void AddRepoWeek(RepoWeek week)
        {
            repoWeeks = repoWeeks.Append(week).ToArray();
        }
    }

    public class RepoWeek
    {
        public int weekNumber;
        public int yearNumber;
        public RepoCommit[] commits;

        public RepoWeek(int weekNumber, int yearNumber)
        {
            this.weekNumber = weekNumber;
            this.yearNumber = yearNumber;
            commits = new RepoCommit[0];
        }

        public void AddCommit(RepoCommit commit)
        {
            commits = commits.Append(commit).ToArray();
        }
    }

    public class RepoCommit
    {
        public string commitId;
        public string message;
        public DateTime date;
        public bool selected = false;

        public RepoDirectory[] directories;
        public string[] files;
    }

    public class RepoDirectory
    {
        public string name;
        public RepoDirectory[] directories;
        public string[] files;

        public RepoDirectory(string name)
        {
            this.name = name;
            files = new string[0];
            directories = new RepoDirectory[0];
        }

        public RepoDirectory AddDirectory(string name)
        {
            foreach(RepoDirectory dir in directories)
            {
                if (dir.name == name)
                {
                    return dir;
                }
            }

            RepoDirectory newDir = new RepoDirectory(name);

            directories = directories.Append(newDir).ToArray();

            return newDir;
        }

        public void AddFile(string fileName)
        {
            files = files.Append(fileName).ToArray();
        }


        public RepoDirectory GetDirectory(string name)
        {
            for(int i = 0; i < directories.Length; i++)
            {
                if(directories[i].name == name)
                {
                    return directories[i];
                }
            }

            return null;
        }
    }
}
