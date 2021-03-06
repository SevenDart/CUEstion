using System;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WEB.Controllers
{
    [Authorize]
    [ApiController]
    [Route("workspaces/{workspaceId}/roles")]
    public class WorkspaceRolesController: ControllerBase
    {
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;
        private readonly IWorkspaceManagerService _workspaceManagerService;

        public WorkspaceRolesController(
            IWorkspaceRoleManagerService workspaceRoleManagerService, 
            IWorkspaceManagerService workspaceManagerService)
        {
            _workspaceRoleManagerService = workspaceRoleManagerService;
            _workspaceManagerService = workspaceManagerService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetWorkspaceRoles(int workspaceId)
        {
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }
            
            var accessingUserId = Tools.GetUserIdFromToken(User);
            if (accessingUserId == null 
                || !await _workspaceRoleManagerService.CheckUserAccess(accessingUserId.Value, workspaceId))
            {
                return Forbid();
            }

            var roles = await _workspaceRoleManagerService.GetAllWorkspaceRoles(workspaceId);
            return Ok(roles);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetWorkspaceRoleById(int workspaceId, int roleId)
        {
            var workspace = await _workspaceManagerService.GetWorkspaceById(workspaceId);
            if (workspace == null)
            {
                return NotFound();
            }
            
            var accessingUserId = Tools.GetUserIdFromToken(User);
            if (accessingUserId == null 
                || !await _workspaceRoleManagerService.CheckUserAccess(accessingUserId.Value, workspaceId))
            {
                return Forbid();
            }

            var role = await _workspaceRoleManagerService.GetWorkspaceRoleById(roleId);
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspaceRole(
            int workspaceId,
            [FromBody] WorkspaceRoleDto workspaceRoleDto)
        {
            var accessingUserId = Tools.GetUserIdFromToken(User);
            if (accessingUserId == null 
                || !await _workspaceRoleManagerService.CheckUserAccess(accessingUserId.Value, workspaceId, AccessRights.CanManageRoles))
            {
                return Forbid();
            }

            var result = await _workspaceRoleManagerService.CreateWorkspaceRole(workspaceRoleDto);
            return Ok(result);
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateWorkspaceRole(
            int workspaceId, 
            int roleId, 
            [FromBody] WorkspaceRoleDto workspaceRoleDto)
        {
            var accessingUserId = Tools.GetUserIdFromToken(User);
            if (accessingUserId == null 
                || !await _workspaceRoleManagerService.CheckUserAccess(accessingUserId.Value, workspaceId, AccessRights.CanManageRoles))
            {
                return Forbid();
            }

            var result = await _workspaceRoleManagerService.UpdateWorkspaceRole(roleId, workspaceRoleDto);
            return Ok(result);
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteWorkspaceRole(int workspaceId, int roleId)
        {
            var accessingUserId = Tools.GetUserIdFromToken(User);
            if (accessingUserId == null 
                || !await _workspaceRoleManagerService.CheckUserAccess(accessingUserId.Value, workspaceId, AccessRights.CanManageRoles))
            {
                return Forbid();
            }

            try
            {
                await _workspaceRoleManagerService.DeleteWorkspaceRole(roleId);
            }
            catch (InvalidOperationException)
            {
                return Conflict("There are users with this role, change their roles first to delete role.");
            }
            catch (DbUpdateException)
            {
                return Conflict("There are users with this role, change their roles first to delete role.");
            }
            return Ok();
        }
    }
}