using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using BLL.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [Authorize]
    [ApiController]
    [Route("workspaces")]
    public class WorkspacesController : ControllerBase
    {
        private readonly IWorkspaceManagerService _workspaceManagerService;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;

        public WorkspacesController(
            IWorkspaceManagerService workspaceManagerService, 
            IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _workspaceManagerService = workspaceManagerService;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }
        
        [HttpGet("/users/workspaces")]
        public async Task<IActionResult> GetUserWorkspaces()
        {
            var userId = Tools.GetUserIdFromToken(User);

            if (userId == null)
            {
                return Forbid();
            }
            
            var workspaces = await _workspaceManagerService.GetUserWorkspaces(userId.Value);
            
            return Ok(workspaces);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateWorkspace([FromBody] WorkspaceDto workspaceDto)
        {
            var userId = Tools.GetUserIdFromToken(User);
            
            if (userId == null)
            {
                return Forbid();
            }

            var result = await _workspaceManagerService.CreateWorkspace(userId.Value, workspaceDto);

            return Ok(result);
        }
        
        [HttpPut("{workspaceId}")]
        public async Task<IActionResult> UpdateWorkspace(int workspaceId, [FromBody] WorkspaceDto workspaceDto)
        {
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }
            
            var userId = Tools.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Forbid();
            }

            WorkspaceDto result = null;
            try
            {
                result = await _workspaceManagerService.UpdateWorkspace(workspaceId, userId.Value, workspaceDto);
            }
            catch (ErrorRequestException e)
            {
                return StatusCode(e.ErrorCode, e.Message);
            }

            return Ok(result);
        }
        
        [HttpDelete("{workspaceId}")]
        public async Task<IActionResult> DeleteWorkspace(int workspaceId)
        {
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }
            
            var userId = Tools.GetUserIdFromToken(User);
            if (userId == null)
            {
                return Forbid();
            }

            try
            {
                await _workspaceManagerService.DeleteWorkspace(workspaceId, userId.Value);
            }
            catch (ErrorRequestException e)
            {
                return StatusCode(e.ErrorCode, e.Message);
            }

            return Ok();
        }

        [HttpPut("{workspaceId}/add-user")]
        public async Task<IActionResult> AddUserToWorkspace(int workspaceId, [FromBody] WorkspaceUserDto workspaceUserDto)
        {
            var addingUserId = Tools.GetUserIdFromToken(User);
            if (addingUserId == null)
            {
                return Forbid();
            }
            
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }

            if (!await _workspaceRoleManagerService.CheckUserAccess(addingUserId.Value, workspaceId,
                AccessRights.CanAddUsers))
            {
                return Forbid();
            }

            try
            {
                await _workspaceManagerService.AddUserToWorkspace(workspaceId, workspaceUserDto.UserId,
                    workspaceUserDto.WorkspaceRoleId);
            }
            catch (ErrorRequestException e)
            {
                return StatusCode(e.ErrorCode, e.Message);
            }

            return Ok();
        }

        [HttpDelete("{workspaceId}/remove-user/{userId}")]
        public async Task<IActionResult> RemoveUserFromWorkspace(int workspaceId, int userId)
        {
            var addingUserId = Tools.GetUserIdFromToken(User);
            if (addingUserId == null)
            {
                return Forbid();
            }
            
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }

            if (!await _workspaceRoleManagerService.CheckUserAccess(addingUserId.Value, workspaceId,
                AccessRights.CanAddUsers))
            {
                return Forbid();
            }

            try
            {
                await _workspaceManagerService.RemoveUserFromWorkspace(workspaceId, userId);
            }
            catch (ErrorRequestException e)
            {
                return StatusCode(e.ErrorCode, e.Message);
            }

            return Ok();
        }

        [HttpPut("{workspaceId}/update-user/{userId}")]
        public async Task<IActionResult> UpdateWorkspaceUser(int workspaceId, int userId, [FromBody] WorkspaceUserDto workspaceUserDto)
        {
            var addingUserId = Tools.GetUserIdFromToken(User);
            if (addingUserId == null)
            {
                return Forbid();
            }
            
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }

            if (!await _workspaceRoleManagerService.CheckUserAccess(addingUserId.Value, workspaceId,
                AccessRights.CanAddUsers))
            {
                return Forbid();
            }

            try
            {
                await _workspaceManagerService.UpdateWorkspaceUser(workspaceId, userId, workspaceUserDto.WorkspaceRoleId);
            }
            catch (ErrorRequestException e)
            {
                return StatusCode(e.ErrorCode, e.Message);
            }

            return Ok();
        }

        [HttpGet("{workspaceId}/users")]
        public async Task<IActionResult> GetAllWorkspaceUsers(int workspaceId)
        {
            var addingUserId = Tools.GetUserIdFromToken(User);
            if (addingUserId == null)
            {
                return Forbid();
            }
            
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }

            if (!await _workspaceRoleManagerService.CheckUserAccess(addingUserId.Value, workspaceId))
            {
                return Forbid();
            }

            var users = await _workspaceManagerService.GetAllWorkspaceUsers(workspaceId);
            return Ok(users);
        }
    }
}