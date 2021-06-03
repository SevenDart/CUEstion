using System;
using CUEstion.BLL;
using CUEstion.BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("questions")]
	public class QuestionsController : ControllerBase
	{

		private QuestionManagerService _questionManagerService;

		QuestionsController()
		{
			_questionManagerService = new QuestionManagerService();
		}

		[HttpGet]
		public IActionResult GetAllQuestions()
		{
			try
			{
				var list = _questionManagerService.GetAllQuestions();
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("filter")]
		public IActionResult GetQuestionsByTags(string[] tags)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpGet("{questionId}")]
		public IActionResult GetQuestion(int questionId)
		{
			try
			{
				var question = _questionManagerService.GetQuestion(questionId);
				return Ok(question);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut]
		//[Authorize]
		public IActionResult CreateQuestion(QuestionDTO questionDto)
		{
			try
			{
				_questionManagerService.CreateQuestion(questionDto);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}")]
		[Authorize]
		public IActionResult DeleteQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}")]
		[Authorize]
		public IActionResult UpdateQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("{questionId}/subscribe")]
		[Authorize]
		public IActionResult SubscribeToQuestion()
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers")]
		[Authorize]
		public IActionResult CreateAnswer(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult UpdateAnswer(int questionId, int answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpDelete("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult DeleteAnswer(int questionId, int answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/comments")]
		[Authorize]
		public IActionResult CreateCommentToQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}/comments")]
		[Authorize]
		public IActionResult CreateCommentToAnswer(int questionId, int answerId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}")]
		[Authorize]
		public IActionResult VoteForQuestion(int questionId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPut("{questionId}/answers/{answerId}")]
		[Authorize]
		public IActionResult VoteForAnswer(int questionId, int answerId)
		{
			try
			{
				int mark = int.Parse(Request.Query["mark"]);
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



		[HttpPut("{questionId}/comments/{commentId}")]
		[Authorize]
		public IActionResult VoteForCommentToQuestion(int questionId, int commentId, int mark)
		{
			try
			{	

				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpPut("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult VoteForCommentToAnswer(int questionId, int answerId, int commentId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpDelete("{questionId}/answers/{answerId}/comments/{commentId}")]
		[Authorize]
		public IActionResult DeleteCommentToAnswer(int questionId, int answerId, int commentId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpDelete("{questionId}/answers/comments/{commentId}")]
		[Authorize]
		public IActionResult DeleteCommentToQuestion(int questionId, int commentId)
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



	}
}
