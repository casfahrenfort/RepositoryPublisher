using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThesisPrototype.Models;
using ThesisPrototype.Services.Interfaces;

namespace ThesisPrototype.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishingController : ControllerBase
    {
        private readonly IPublishingService publishingService;
        private readonly IDraftingService draftingService;
        private readonly IGitHubStreamService gitHubStreamService;
        private readonly ISubversionStreamService subversionStreamService;

        public PublishingController(IPublishingService publishingService,
            IDraftingService draftingService,
            IGitHubStreamService gitHubStreamService,
            ISubversionStreamService subversionStreamService)
        {
            this.publishingService = publishingService;
            this.draftingService = draftingService;
            this.gitHubStreamService = gitHubStreamService;
            this.subversionStreamService = subversionStreamService;
        }
        
        [HttpPost]
        [Route("github")]
        public async Task<ActionResult<string>> GitHub([FromBody] MetaData metaData, [FromQuery] string author, [FromQuery] string repo)
        {
            HttpResponseMessage response = await draftingService.CreateDraftRecord(metaData);

            if(!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }

            string jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            string recordId = jsonResponse.id;
            string fileBucketId = jsonResponse.links.files;
            fileBucketId = fileBucketId.Split('/').Last();

            Stream gitHubStream = gitHubStreamService.GitFolderStream("https://github.com/casfahrenfort/gwtemplatecoder");

            response = await draftingService.UploadStreamToDraftRecord(gitHubStream, ".git.tar.gz", fileBucketId);
            
            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await draftingService.DeleteDraftRecord(recordId);
                return response.StatusCode.ToString();
            }

            response = await publishingService.PublishDraftRecord(recordId);
            
            if (!response.IsSuccessStatusCode)
            {
                // Delete draft to ensure transactional nature of request
                await draftingService.DeleteDraftRecord(recordId);
                return response.StatusCode.ToString();
            }

            return response.StatusCode.ToString();
        }

        [HttpPost]
        [Route("svn")]
        public async Task<ActionResult<string>> Svn([FromQuery] string svnUrl)
        {
            this.subversionStreamService.SvnStream(svnUrl);

            return "";
        }

        [HttpPost]
        [Route("binary")]
        public async Task<ActionResult<string>> Binary()
        {
            var x = Request.Form.Files;

            return "";
        }
    }
}
