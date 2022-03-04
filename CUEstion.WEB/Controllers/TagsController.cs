using System;
using System.Threading.Tasks;
using CUEstion.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CUEstion.WEB.Controllers
{
    public class TagsController : ControllerBase
    {
        private readonly ITagManagerService _tagManagerService;

        public TagsController(ITagManagerService tagManagerService)
        {
            _tagManagerService = tagManagerService;
        }
        
        [HttpGet("tags")]
        public async Task<IActionResult> GetAllTags()
        {
            var list = await _tagManagerService.GetAllTags();
            return Ok(list);		
        }
        
        [HttpPost("tags")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateTag([FromQuery] string tag)
        {
            tag = Uri.UnescapeDataString(tag);
            await _tagManagerService.CreateTag(tag);
            return Ok();
        }

        [HttpPut("tags")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateTag([FromQuery] string oldTag, string newTag)
        {
            oldTag = Uri.UnescapeDataString(oldTag);
            newTag = Uri.UnescapeDataString(newTag);
            await _tagManagerService.UpdateTag(oldTag, newTag);
            return Ok();
        }


        [HttpDelete("tags")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteTag([FromQuery] string tag)
        {
            tag = Uri.UnescapeDataString(tag);
            await _tagManagerService.DeleteTag(tag);
            return Ok();
        }
    }
}