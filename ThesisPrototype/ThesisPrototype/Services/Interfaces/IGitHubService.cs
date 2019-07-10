using LibGit2Sharp;
using System.Collections.Generic;
using System.IO;
using ThesisPrototype.Models;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IGitHubService
    {
        List<DraftFile> GitFolderAndCommitStreams(string gitHubUrl);
    }
}
