﻿using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BLL.Interfaces;

namespace WEB.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagerService _userManagerService;
        private readonly IQuestionManagerService _questionManagerService;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;

        public UsersController(IUserManagerService userManagerService, 
            IQuestionManagerService questionManagerService, 
            IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _userManagerService = userManagerService;
            _questionManagerService = questionManagerService;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var list = await _userManagerService.GetAllUsers();
            return Ok(list);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _userManagerService.GetUser(userId);
            return user != null
                ? Ok(user)
                : NotFound(new {Message = "No such user."});
        }


        [HttpPut("login")]
        public async Task<IActionResult> Login(AuthDto authDto)
        {
            authDto = await _userManagerService.CheckAuthData(authDto);
            
            if (authDto == null)
                return StatusCode(401, "There is no user with such email and password");

            return Ok(new
            {
                token = Tools.CreateToken(authDto.Email, authDto.Id, authDto.Role),
                role = authDto.Role,
                id = authDto.Id,
                expirationTime = DateTime.Now.AddHours(AuthOptions.LIFETIME)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthDto authDto)
        {
            authDto = await _userManagerService.CreateUser(authDto);

            return Ok(new
            {
                token = Tools.CreateToken(authDto.Email, authDto.Id, authDto.Role),
                role = authDto.Role,
                id = authDto.Id,
                expirationTime = DateTime.Now.AddHours(AuthOptions.LIFETIME)
            });
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userManagerService.DeleteUser(userId);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "User not found."});
            }

            return Ok();
        }


        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserInfo(UserDto userDto)
        {
            try
            {
                await _userManagerService.UpdateUserInfo(userDto);
            }
            catch (NullReferenceException)
            {
                return NotFound(new {Message = "User not found."});
            }

            return Ok();
        }


        [HttpGet("{userId}/questions")]
        public async Task<IActionResult> GetUsersQuestions(int userId, [FromQuery] int? workspaceId)
        {
            if (workspaceId != null)
            {
                var accessingUser = Tools.GetUserIdFromToken(User);
                if (accessingUser == null || 
                    !await _workspaceRoleManagerService.CheckUserAccess(accessingUser.Value, workspaceId.Value))
                {
                    return Forbid();
                }
            }
            
            var list = await _questionManagerService.GetQuestionsCreatedByUser(userId, workspaceId);
            return Ok(list);
        }
    }
}