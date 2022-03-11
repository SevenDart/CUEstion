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
        private readonly IMarkManagerService _markManagerService;

        public QuestionsController(IQuestionManagerService questionManagerService, IMarkManagerService markManagerService)
        {
            _questionManagerService = questionManagerService;
            _markManagerService = markManagerService;
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
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            return question != null
                ? Ok(question)
                : NotFound(new {Message = $"Question with id {questionId} not found."});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateQuestion(QuestionDto questionDto)
        {
            await _questionManagerService.CreateQuestion(questionDto);
            return Ok(questionDto);
        }


        [HttpDelete("{questionId}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuestion(int questionId)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            await _questionManagerService.DeleteQuestion(questionId);
            return Ok();
        }

        [HttpPut("{questionId}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuestion(QuestionDto questionDto)
        {
            var question = await _questionManagerService.GetQuestionAsync(questionDto.Id);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionDto.Id} not found."});
            }

            await _questionManagerService.UpdateQuestion(questionDto);
            return Ok(questionDto);
        }


        [HttpPut("{questionId}/upvote")]
        [Authorize]
        public async Task<IActionResult> UpvoteForQuestion(int questionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);

            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId, questionId);
            if (currentMark != null && currentMark.MarkValue == 1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _questionManagerService.MarkQuestion(userId, questionId, 1);

            return Ok();
        }

        [HttpPut("{questionId}/downvote")]
        [Authorize]
        public async Task<IActionResult> DownvoteForQuestion(int questionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);
            
            var question = await _questionManagerService.GetQuestionAsync(questionId);
            if (question == null)
            {
                return NotFound(new {Message = $"Question with id {questionId} not found."});
            }
            
            var currentMark = await _markManagerService.GetQuestionMarkAsync(userId, questionId);
            if (currentMark != null &&  currentMark.MarkValue == -1)
            {
                return Conflict(new { Message = "You can't set the same mark again." });
            }
            
            await _questionManagerService.MarkQuestion(userId, questionId, -1);

            return Ok();
        }
    }
}