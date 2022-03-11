using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BLL.Interfaces;
using BLL.ModelsDTO;

namespace WEB.Controllers
{
    [ApiController]
    [Route("questions")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionManagerService _questionManagerService;

        public QuestionsController(IQuestionManagerService questionManagerService)
        {
            _questionManagerService = questionManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions()
        {
            var list = await _questionManagerService.GetAllQuestions();
            return Ok(list);
        }

        [HttpGet("Hot")]
        public async Task<IActionResult> GetHotQuestions(int count)
        {
            var list = await _questionManagerService.GetNewestQuestions(count);
            return Ok(list);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetQuestionsByTags([FromQuery] string[] tags)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = Uri.UnescapeDataString(tags[i]);
            }

            var list = await _questionManagerService.FilterQuestions(tags);
            return Ok(list);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query, [FromQuery] string[] tags)
        {
            if (query == null) query = "";
            for (int i = 0; i < tags.Length; i++)
            {
                tags[i] = Uri.UnescapeDataString(tags[i]);
            }

            var list = await _questionManagerService.Search(query, tags);
            return Ok(list);
        }

        [HttpGet("{questionId}")]
        public async Task<IActionResult> GetQuestion(int questionId)
        {
            var question = await _questionManagerService.GetQuestion(questionId);
            return question != null
                ? Ok(question)
                : StatusCode(404, new {Message = "No such question."});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateQuestion(QuestionDto questionDto)
        {
            try
            {
                await _questionManagerService.CreateQuestion(questionDto);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "Tag not found."});
            }

            return Ok(questionDto.Id);
        }


        [HttpDelete("{questionId}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            try
            {
                await _questionManagerService.DeleteQuestion(questionId);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "No such question."});
            } 
            
            return Ok();
        }

        [HttpPut("{questionId}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(QuestionDto questionDto)
        {
            try
            {
                await _questionManagerService.UpdateQuestion(questionDto);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "No such question."});
            }

            return Ok();
        }


        [HttpPut("{questionId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForQuestion(int questionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            try
            {
                await _questionManagerService.MarkQuestion(userId, questionId, 1);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "No such question."});
            }

            return Ok();
        }

        [HttpPut("{questionId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForQuestion(int questionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            try
            {
                await _questionManagerService.MarkQuestion(userId, questionId, -1);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "No such question."});
            }

            return Ok();
        }
    }
}