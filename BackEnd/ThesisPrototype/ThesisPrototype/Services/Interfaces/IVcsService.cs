using ThesisPrototype.Models;
using ThesisPrototype.Models.Repo;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IVcsService
    {
        Snapshot GetRepositorySnapshot(string url, string repoName, string requestId);

        void DeleteRequestDirectory(string requestId);

        RepoTree GetRepositoryTree(string url);
    }
}
