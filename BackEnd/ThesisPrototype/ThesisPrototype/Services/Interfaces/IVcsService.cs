using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IVcsService
    {
        Snapshot GetRepositorySnapshot(string url, string repoName, string requestId);

        void DeleteRequestDirectory(string requestId);
    }
}
