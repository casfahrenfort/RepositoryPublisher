using System.IO;

namespace ThesisPrototype.Services.Interfaces
{
    public interface IGitHubStreamService
    {
        Stream GitFolderStream(string gitHubUrl);
    }
}
