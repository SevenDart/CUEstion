using CUEstion.BLL;
using CUEstion.BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CUEstion.WEB.Controllers
{
	[ApiController]
	[Route("users")]
	public class UsersController : ControllerBase
	{

		private UserManagerService _userManagerService;

		public UsersController()
		{
			_userManagerService = new UserManagerService();
		}

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


		[HttpPut]
		public IActionResult Login(AuthDTO authDto)
		{
			try
			{
				authDto = _userManagerService.CheckAuthData(authDto);
				if (authDto == null)
					return StatusCode(401, "There is no user with such email and password");

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, authDto.Email),
					new Claim(ClaimTypes.Sid, authDto.Id.ToString()),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, authDto.Role)
				};

				var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimTypes.Email, ClaimTypes.Role);

				var now = DateTime.UtcNow;
				var jwt = new JwtSecurityToken(
					issuer: AuthOptions.ISSUER,
					audience: AuthOptions.AUDIENCE,
					notBefore: now,
					claims: claimsIdentity.Claims,
					expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFETIME)),
					signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
					SecurityAlgorithms.HmacSha256)
				);

				var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

				return Ok(new
				{
					token = encodedJwt,
					role = authDto.Role
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

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, authDto.Email),
					new Claim(ClaimTypes.Sid, authDto.Id.ToString()),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, authDto.Role)
				};

				var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimTypes.Email, ClaimTypes.Role);

				var now = DateTime.UtcNow;
				var jwt = new JwtSecurityToken(
					issuer: AuthOptions.ISSUER,
					audience: AuthOptions.AUDIENCE,
					notBefore: now,
					claims: claimsIdentity.Claims,
					expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFETIME)),
					signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), 
					SecurityAlgorithms.HmacSha256)
				);

				var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

				return Ok(new {
					token = encodedJwt,
					role = authDto.Role
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
				return Ok();
			}
			catch (Exception e)
			{
				
				return StatusCode(500, new { Message = "Server ERROR occured." });
			}
		}


		[HttpPut("{userId}")]
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
