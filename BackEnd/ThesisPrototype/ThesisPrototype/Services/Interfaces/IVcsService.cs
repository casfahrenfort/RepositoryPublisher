using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IVcsService
    {
        byte[] GetRepositorySnapshot(string url, string repoName);
    }
}
