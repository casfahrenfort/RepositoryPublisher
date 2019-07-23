using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Converters;
using ThesisPrototype.Models;
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
                snapshot = vcsService.GetRepositorySnapshot(publishInfo.repoURL, publishInfo.repoName);
            }
            catch(Exception e)
            {
                return BadRequest(new ErrorResponse()
                {
                    message = $"An error occurred while cloning the repository at {publishInfo.repoURL}. Please make sure the repository is publicly accessible."
                });
            }

            Publication duplicate = publicationService.FindDuplicatePublication(snapshot.checksum);
            if (duplicate != null)
            {
                return BadRequest(new ErrorResponse()
                {
                    message = $"This repository has already been published at {duplicate.PublicationUrl}"
                });
            }

            IPublishingService publishingService = b2ShareService;

            HttpResponseMessage response = await publishingService.PublishRepository(snapshot.zippedBytes, publishInfo.repoName, publishInfo.metaData);

            string jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new PublishErrorResponse()
                {
                    message = "An error occurred while publishing files to B2SHARE. See B2SHARE response for more detail.",
                    b2ShareResponse = new B2ShareResponse()
                    {
                        message = (string)jsonResponse.message,
                        status = (string)jsonResponse.status
                    }
                });
            }

            publicationService.CreatePublication(publishInfo.repoURL, (string)jsonResponse.links.publication, publishInfo.metaData.open_access, snapshot.checksum);

            return Ok(new PublishResponse()
            {
                message = "Repository successfully published.",
                publishUrl = jsonResponse.links.publication
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
