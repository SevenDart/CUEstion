using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("users")]
	public class UsersController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetAllUsers()
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

		[HttpGet("{userId}")]
		public IActionResult GetUser(int userId)
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


		[HttpPut("register")]
		public IActionResult Register()
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

		[HttpDelete("{userId}")]
		[Authorize]
		public IActionResult DeleteUser(int userId)
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


		[HttpPost("{userId}")]
		[Authorize]
		public IActionResult UpdateUserInfo(int userId)
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


		[HttpGet("{userId}/questions")]
		public IActionResult GetUsersQuestions(int userId)
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
