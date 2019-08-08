using Microsoft.AspNetCore.Mvc;
using ThesisPrototype.Models;
using ThesisPrototype.Models.Repo;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepoController : ControllerBase
    {
        private readonly IB2ShareService b2ShareService;
        private readonly IGitHubService gitHubService;
        private readonly ISubversionService subversionService;
        private readonly IPublicationService publicationService;

        public RepoController(IB2ShareService b2ShareService,
            IGitHubService gitHubService,
            ISubversionService subversionService,
            IPublicationService publicationService)
        {
            this.b2ShareService = b2ShareService;
            this.gitHubService = gitHubService;
            this.subversionService = subversionService;
            this.publicationService = publicationService;
        }

        [HttpPost]
        public ActionResult<RepoTree> GetRepositoryTree([FromBody] RepoInfo repoInfo)
        {
            IVcsService vcsService;
            switch (repoInfo.versionControl)
            {
                case "git": vcsService = gitHubService; break;
                case "svn": vcsService = subversionService; break;
                default: vcsService = gitHubService; break;
            }

            return Ok(vcsService.GetRepositoryTree(repoInfo.repoUrl));
        }
    }
}