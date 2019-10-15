using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishingController : ControllerBase
    {
        private readonly IB2ShareService b2ShareService;
        private readonly IFigshareService figshareService;
        private readonly IDataverseService dataverseService;
        private readonly IGitHubService gitHubService;
        private readonly ISubversionService subversionService;
        private readonly IPublicationService publicationService;

        public PublishingController(IB2ShareService b2ShareService,
            IFigshareService figshareService,
            IDataverseService dataverseService,
            IGitHubService gitHubService,
            ISubversionService subversionService,
            IPublicationService publicationService)
        {
            this.b2ShareService = b2ShareService;
            this.figshareService = figshareService;
            this.gitHubService = gitHubService;
            this.subversionService = subversionService;
            this.publicationService = publicationService;
            this.dataverseService = dataverseService;
        }

        [HttpPost]
        public async Task<ActionResult<PublishResponse>> PublishRepository([FromBody] PublishInfo publishInfo)
        {
            IVcsService vcsService;
            switch (publishInfo.versionControl)
            {
                case "git": vcsService = gitHubService; break;
                case "svn": vcsService = subversionService; break;
                default: vcsService = gitHubService; break;
            }

            Snapshot snapshot;
            try
            {
                snapshot = vcsService.GetRepositorySnapshot(publishInfo.repoURL, publishInfo.repoName, publishInfo.snapshotId, HttpContext.TraceIdentifier.Replace(':', '.'));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse()
                {
                    message =  $"An error occurred while accessing the repository at {publishInfo.repoURL}. Please make sure the repository is publicly accessible."
                });
            }

            Publication duplicate = publicationService.FindDuplicatePublication(snapshot.checksum);
            if (duplicate != null)
            {
                return BadRequest(new DuplicatePublicationResponse()
                {
                    message = $"This repository has already been published.",
                    duplicatePublicationUrl = duplicate.PublicationUrl
                });
            }

            IPublishingService publishingService;
            switch (publishInfo.publishingSystem)
            {
                case "b2share": publishingService = b2ShareService; break;
                case "figshare": publishingService = figshareService; break;
                case "dataverse": publishingService = dataverseService; break;
                default: publishingService = b2ShareService; break;
            }

            Response response = await publishingService.PublishRepository(snapshot.zippedBytes, publishInfo);

            if (response is PublishResponse)
            {
                publicationService.CreatePublication(publishInfo.repoURL, ((PublishResponse)response).publishUrl, publishInfo.metaData.open_access, snapshot.checksum);
                
                return Ok(response);
            }
            else if (response is PublishErrorResponse)
            {
                return BadRequest(response);
            }

            return BadRequest(new ErrorResponse()
            {
                message = "Unknown error occurred."
            });
        }

        [HttpPost]
        [Route("multiple")]
        public async Task<ActionResult<List<PublishResponse>>> PublishRepositoryBundle([FromBody] PublishInfo[] publishInfos)
        {
            List<PublishInfo> publicationsToMake = new List<PublishInfo>();
            string[] publicationIds = new string[0];
            string[] publicationUrls = new string[0];
            List<Publication> duplicates = new List<Publication>();

            for (int i = 0; i < publishInfos.Length - 1; i++)
            {
                IVcsService vcsService;
                switch (publishInfos[i].versionControl)
                {
                    case "git": vcsService = gitHubService; break;
                    case "svn": vcsService = subversionService; break;
                    default: vcsService = gitHubService; break;
                }

                Snapshot snapshot;
                try
                {
                    snapshot = vcsService.GetRepositorySnapshot(publishInfos[i].repoURL, publishInfos[i].repoName, publishInfos[i].snapshotId, HttpContext.TraceIdentifier.Replace(':', '.'));
                }
                catch
                {
                    return BadRequest(new ErrorResponse()
                    {
                        message = $"An error occurred while accessing the repository at {publishInfos[i].repoURL}. Please make sure the repository is publicly accessible."
                    });
                }

                Publication duplicate = publicationService.FindDuplicatePublication(snapshot.checksum);
                if (duplicate != null)
                {
                    duplicates.Add(duplicate);
                    publicationIds = publicationIds.Append(duplicate.Id).ToArray();
                    publicationUrls = publicationUrls.Append(duplicate.PublicationUrl).ToArray();
                }
                else
                {
                    publishInfos[i].snapshot = snapshot;
                    publicationsToMake.Add(publishInfos[i]);
                }
            }

            // All repositories have been published already
            if (duplicates.Count == publishInfos.Length - 1)
            {
                PublicationBundle duplicateBundle = publicationService.FindDuplicatePublicationBundle(duplicates);

                if (duplicateBundle != null)
                {
                    return BadRequest(new DuplicatePublicationResponse()
                    {
                        message = $"A similar bundle has already been published.",
                        duplicatePublicationUrl = duplicateBundle.PublicationUrl
                    });
                }
            }

            PublishInfo bundlePublishInfo = publishInfos.Last();

            IPublishingService publishingService;
            switch (bundlePublishInfo.publishingSystem)
            {
                case "b2share": publishingService = b2ShareService; break;
                case "figshare": publishingService = figshareService; break;
                case "dataverse": publishingService = dataverseService; break;
                default: publishingService = b2ShareService; break;
            }

            Response response = await publishingService.PublishMultipleRepositories(publicationsToMake, duplicates, bundlePublishInfo);

            if (response is MultiplePublishResponse)
            {
                MultiplePublishResponse publishResponse = (MultiplePublishResponse)response;
                foreach (PublishingSystemPublication pubSystemPublication in publishResponse.bundlePublicationInfos)
                {
                    Publication publication = publicationService.CreatePublication(pubSystemPublication.publishInfo.repoURL, pubSystemPublication.publicationUrl, pubSystemPublication.publishInfo.metaData.open_access, pubSystemPublication.publishInfo.snapshot.checksum);
                    publicationIds = publicationIds.Append(publication.Id).ToArray();
                    publicationUrls = publicationUrls.Append(publication.PublicationUrl).ToArray();
                }

                PublicationBundle publicationBundle = publicationService.CreatePublicationBundle(publicationUrls, publicationIds, publishResponse.bundlePublicationUrl, bundlePublishInfo.metaData.open_access);

                return Ok(new PublishResponse
                {
                    message = "Bundle successfully published.",
                    publishUrl = publicationBundle.PublicationUrl
                });

            }
            else if (response is PublishErrorResponse)
            {
                return BadRequest(response);
            }

            return BadRequest(new ErrorResponse()
            {
                message = "Unknown error occurred."
            });
        }
    }
}
