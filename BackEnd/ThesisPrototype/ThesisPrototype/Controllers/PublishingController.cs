using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Models.B2Share;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishingController : ControllerBase
    {
        private readonly IB2ShareService b2ShareService;
        private readonly IGitHubService gitHubService;
        private readonly ISubversionService subversionService;
        private readonly IPublicationService publicationService;

        public PublishingController(IB2ShareService b2ShareService,
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
                snapshot = vcsService.GetRepositorySnapshot(publishInfo.repoURL, publishInfo.repoName, HttpContext.TraceIdentifier.Replace(':', '.'));
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

            IPublishingService publishingService = b2ShareService;

            Response response = await publishingService.PublishRepository(snapshot.zippedBytes, publishInfo.repoName, publishInfo.metaData);

            if (response is B2SharePublishResponse)
            {
                publicationService.CreatePublication(publishInfo.repoURL, ((B2SharePublishResponse)response).publicationUrl, publishInfo.metaData.open_access, snapshot.checksum);

                return Ok(new PublishResponse
                {
                    message = "Repository successfully published.",
                    publishUrl = ((B2SharePublishResponse)response).publicationUrl
                });
            }
            else if (response is B2ShareResponse)
            {
                return BadRequest(new PublishErrorResponse()
                {
                    message = "An error occurred while publishing files to B2SHARE. See B2SHARE response for more detail.",
                    b2ShareResponse = (B2ShareResponse)response
                });
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
                    snapshot = vcsService.GetRepositorySnapshot(publishInfos[i].repoURL, publishInfos[i].repoName, HttpContext.TraceIdentifier.Replace(':', '.'));
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

            IPublishingService publishingService = b2ShareService;

            Response response = await publishingService.PublishMultipleRepositories(publicationsToMake, bundlePublishInfo);

            if (response is B2ShareMultiplePublishResponse)
            {
                B2ShareMultiplePublishResponse publishResponse = (B2ShareMultiplePublishResponse)response;
                foreach (B2SharePublication b2SharePublication in publishResponse.bundlePublicationInfos)
                {
                    Publication publication = publicationService.CreatePublication(b2SharePublication.publishInfo.repoURL, b2SharePublication.publicationUrl, b2SharePublication.publishInfo.metaData.open_access, b2SharePublication.publishInfo.snapshot.checksum);
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
            else if (response is B2ShareResponse)
            {
                return BadRequest(new PublishErrorResponse()
                {
                    message = "An error occurred while publishing files to B2SHARE. See B2SHARE response for more detail.",
                    b2ShareResponse = (B2ShareResponse)response
                });
            }

            return BadRequest(new ErrorResponse()
            {
                message = "Unknown error occurred."
            });
        }


        [HttpDelete]
        public async Task<ActionResult<string>> Delete()
        {
            HttpResponseMessage response = await b2ShareService.ListAllRecords();

            string jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Delete every existing draft record
            foreach (dynamic hit in jsonResponse.hits.hits)
            {
                response = await b2ShareService.DeleteDraftRecord((string)hit.id);
            }

            return "";
        }
    }
}
