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
        private readonly IGitHubService gitHubService;
        private readonly ISubversionStreamService subversionStreamService;

        public PublishingController(IPublishingService publishingService,
            IDraftingService draftingService,
            IGitHubService gitHubService,
            ISubversionStreamService subversionStreamService)
        {
            this.publishingService = publishingService;
            this.draftingService = draftingService;
            this.gitHubService = gitHubService;
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

            List<DraftFile> draftFiles = gitHubService.GitFolderAndCommitStreams("https://github.com/" + author + "/" + repo);
            foreach(DraftFile draftFile in draftFiles)
            {
                response = await draftingService.UploadStreamToDraftRecord(draftFile.bytes, draftFile.name, fileBucketId);

                if (!response.IsSuccessStatusCode)
                {
                    // Delete draft to ensure transactional nature of request
                    await draftingService.DeleteDraftRecord(recordId);

                    jsonString = await response.Content.ReadAsStringAsync();
                    jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    return response.StatusCode.ToString();
                }
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

        [HttpDelete]
        public async Task<ActionResult<string>> Delete()
        {
            HttpResponseMessage response = await draftingService.ListAllRecords();

            string jsonString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonString);

            // Delete every existing draft record
            foreach(dynamic hit in jsonResponse.hits.hits)
            {
                response = await draftingService.DeleteDraftRecord((string)hit.id);
            }

            return "";
        }
    }
}
