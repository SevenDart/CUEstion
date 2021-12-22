using CUEstion.BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using CUEstion.BLL.Interfaces;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("users")]
	public class UsersController : ControllerBase
	{

		private readonly IUserManagerService _userManagerService;

		public UsersController(IUserManagerService userManagerService)
		{
			_userManagerService = userManagerService;
		}

		[HttpGet]
		public IActionResult GetAllUsers()
		{
			try
			{
				var list = _userManagerService.GetAllUsers();
				return Ok(list);
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
				var user = _userManagerService.GetUser(userId);
				return user != null 
					? Ok(user) 
					: StatusCode(404, new { Message = "No such user." });
			}
			catch (Exception e)
			{
				
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpPut("login")]
		public IActionResult Login(AuthDTO authDto)
		{
			try
			{
				authDto = _userManagerService.CheckAuthData(authDto);
				if (authDto == null)
					return StatusCode(401, "There is no user with such email and password");

				return Ok(new {
					token = Tools.CreateToken(authDto.Email, authDto.Id, authDto.Role),
					role = authDto.Role,
					id = authDto.Id,
					expirationTime = DateTime.Now.AddHours(AuthOptions.LIFETIME)
				});
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpPost("register")]
		public IActionResult Register(AuthDTO authDto)
		{
			try
			{
				authDto = _userManagerService.CreateUser(authDto);

				return Ok(new {
					token = Tools.CreateToken(authDto.Email, authDto.Id, authDto.Role),
					role = authDto.Role,
					id = authDto.Id,
					expirationTime = DateTime.Now.AddHours(AuthOptions.LIFETIME)
				});	
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
				_userManagerService.DeleteUser(userId);
				return Ok();
			}
			catch (Exception e)
			{
				
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpPut("{userId}")]
		[Authorize]
		public IActionResult UpdateUserInfo(UserDTO userDto)
		{
			try
			{
				_userManagerService.UpdateUserInfo(userDto);
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
				var list = _userManagerService.GetUsersQuestions(userId);
				return Ok(list);
			}
			catch (Exception e)
			{
				
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}

		[HttpGet("subscribed")]
		[Authorize]
		public IActionResult GetFollowedQuestions()
		{
			try
			{
				int userId = int.Parse(User.FindFirst(ClaimTypes.Sid).Value);
				var list = _userManagerService.GetFollowedQuestions(userId);
				return Ok(list);
			}
			catch (Exception e)
			{
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}



	}
}
