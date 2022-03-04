using System;
using System.Security.Claims;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [ApiController]
    [Route("questions")]
    public class AnswersController : ControllerBase
    {
        private readonly IAnswerManagerService _answerManagerService;

        public AnswersController(IAnswerManagerService answerManagerService)
        {
            _answerManagerService = answerManagerService;
        }

        [HttpGet("{questionId}/answers")]
        public IActionResult GetAnswers(int questionId)
        {
            var list = _answerManagerService.GetAnswers(questionId);
            return Ok(list);
        }

        [HttpPost("{questionId}/answers")]
        [Authorize]
        public IActionResult CreateAnswer(AnswerDto answerDto, int questionId)
        {
            _answerManagerService.CreateAnswer(answerDto, questionId);
            return Ok();
        }

        [HttpPut("{questionId}/answers/{answerId}")]
        [Authorize]
        public IActionResult UpdateAnswer(AnswerDto answerDto)
        {
            _answerManagerService.UpdateAnswer(answerDto);
            return Ok();
        }

        [HttpDelete("{questionId}/answers/{answerId}")]
        [Authorize]
        public IActionResult DeleteAnswer(int answerId)
        {
            _answerManagerService.DeleteAnswer(answerId);
            return Ok();
        }


        [HttpPut("{questionId}/answers/{answerId}/upvote")]
        [Authorize]
        public IActionResult UpvoteForAnswer(int answerId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            _answerManagerService.MarkAnswer(userId, answerId, 1);
            return Ok();
        }

        [HttpPut("{questionId}/answers/{answerId}/downvote")]
        [Authorize]
        public IActionResult DownvoteForAnswer(int answerId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
            _answerManagerService.MarkAnswer(userId, answerId, -1);
            return Ok();
        }
    }
}