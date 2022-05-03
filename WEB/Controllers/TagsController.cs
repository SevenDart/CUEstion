using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [ApiController]
    [Route("Tags")]
    public class TagsController : ControllerBase
    {
        private readonly ITagManagerService _tagManagerService;

        public TagsController(ITagManagerService tagManagerService)
        {
            _tagManagerService = tagManagerService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllTags()
        {
            var list = await _tagManagerService.GetAllTags();
            return Ok(list);		
        }
        
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTag([FromQuery] string tag)
        {
            tag = Uri.UnescapeDataString(tag);
            await _tagManagerService.CreateTag(tag);
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTag([FromQuery] string oldTag, string newTag)
        {
            oldTag = Uri.UnescapeDataString(oldTag);
            newTag = Uri.UnescapeDataString(newTag);
            await _tagManagerService.UpdateTag(oldTag, newTag);
            return Ok();
        }


        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag([FromQuery] string tag)
        {
            tag = Uri.UnescapeDataString(tag);
            await _tagManagerService.DeleteTag(tag);
            return Ok();
        }
    }
}