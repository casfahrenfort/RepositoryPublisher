using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThesisPrototype.Models.Repo
{
    public class RepoTree
    {
        public RepoCommit[] commits;
    }

    public class RepoCommit
    {
        public string commitId;
        public DateTime date;

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
