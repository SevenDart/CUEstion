using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class QuestionsController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetAllQuestions()
		{
			try
			{
				return Ok();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("filter")]
		public IActionResult GetQuestionsByTags()
		{
			try
			{
				var tags = new List<string>();
				foreach (var queryElement in Request.Query)
				{
					if (queryElement.Key.StartsWith("tag"))
						tags.Add(queryElement.Value);
				}
				return Ok();
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpGet("{questionId:int}")]
		public IActionResult GetQuestion(int questionId)
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


		[HttpDelete("{questionId:int}")]
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

		[HttpPut]
		[Authorize]
		public IActionResult CreateQuestion()
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

		[HttpPost("{questionId:int}")]
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

		[HttpGet("{questionId:int}/subscribe")]
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

		[HttpPut("{questionId:int}/answers")]
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

		[HttpPost("{questionId:int}/answers/{answerId:int}")]
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

		[HttpDelete("{questionId:int}/answers/{answerId:int}")]
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

		[HttpPut("{questionId:int}/comments")]
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

		[HttpPut("{questionId:int}/answers/{answerId:int}/comments")]
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

		[HttpOptions("{questionId:int}")]
		[Authorize]
		public IActionResult VoteForQuestion(int questionId)
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

		[HttpOptions("{questionId:int}/answers/{answerId:int}")]
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



		[HttpOptions("{questionId:int}/comments/{commentId:int}")]
		[Authorize]
		public IActionResult VoteForCommentToQuestion(int questionId, int commentId)
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


		[HttpOptions("{questionId:int}/answers/{answerId:int}/comments/{commentId:int}")]
		[Authorize]
		public IActionResult VoteForCommentToAnswer(int questionId, int answerId, int commentId)
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


		[HttpDelete("{questionId:int}/answers/{answerId:int}/comments/{commentId:int}")]
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

		[HttpDelete("{questionId:int}/answers/comments/{commentId:int}")]
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
