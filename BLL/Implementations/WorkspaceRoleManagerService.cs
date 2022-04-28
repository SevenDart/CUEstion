using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using BLL.Tools;
using DAL.EF;
using DAL.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementations
{
    public class WorkspaceRoleManagerService: IWorkspaceRoleManagerService
    {
        private readonly ApplicationContext _context;
        
        public WorkspaceRoleManagerService(ApplicationContext context)
        {
            _context = context;
        }
        
        public async Task<WorkspaceRoleDto[]> GetAllWorkspaceRoles(int workspaceId)
        {
            var roles = await _context
                .WorkspaceRoles
                .Where(wr => wr.WorkspaceId == workspaceId)
                .ToListAsync();

            return roles.Adapt<WorkspaceRoleDto[]>();
        }

        public async Task<WorkspaceRoleDto> GetWorkspaceRoleById(int workspaceRoleId)
        {
            var role = await _context
                .WorkspaceRoles
                .FirstOrDefaultAsync(wr => wr.Id == workspaceRoleId);
            
            return role.Adapt<WorkspaceRoleDto>();
        }

        public async Task<WorkspaceRoleDto> CreateWorkspaceRole(WorkspaceRoleDto workspaceRoleDto)
        {
            WorkspaceRole role = workspaceRoleDto.Adapt<WorkspaceRole>();

            await _context.WorkspaceRoles.AddAsync(role);

            await _context.SaveChangesAsync();

            return role.Adapt<WorkspaceRoleDto>();
        }

        public async Task<WorkspaceRoleDto> UpdateWorkspaceRole(int workspaceRoleId, WorkspaceRoleDto workspaceRoleDto)
        {
            var workspaceRole = await _context
                .WorkspaceRoles
                .FirstOrDefaultAsync(wr => wr.Id == workspaceRoleId);

            workspaceRole.Role = (workspaceRoleDto.Role != workspaceRole.Role)
                ? workspaceRoleDto.Role
                : workspaceRole.Role;
            
            workspaceRole.CanCreate = (workspaceRoleDto.CanCreate != workspaceRole.CanCreate)
                ? workspaceRoleDto.CanCreate
                : workspaceRole.CanCreate;
            
            workspaceRole.CanUpdate = (workspaceRoleDto.CanUpdate != workspaceRole.CanUpdate)
                ? workspaceRoleDto.CanUpdate
                : workspaceRole.CanUpdate;
            
            workspaceRole.CanDelete = (workspaceRoleDto.CanDelete != workspaceRole.CanDelete)
                ? workspaceRoleDto.CanDelete
                : workspaceRole.CanDelete;
            
            workspaceRole.CanManageRoles = (workspaceRoleDto.CanManageRoles != workspaceRole.CanManageRoles)
                ? workspaceRoleDto.CanManageRoles
                : workspaceRole.CanManageRoles;
            
            workspaceRole.CanAddUsers = (workspaceRoleDto.CanAddUsers != workspaceRole.CanAddUsers)
                ? workspaceRoleDto.CanAddUsers
                : workspaceRole.CanAddUsers;

            await _context.SaveChangesAsync();

            return workspaceRole.Adapt<WorkspaceRoleDto>();
        }

        public async Task DeleteWorkspaceRole(int workspaceRoleId)
        {
            var workspaceRole = await _context
                .WorkspaceRoles
                .FirstOrDefaultAsync(wr => wr.Id == workspaceRoleId);

            _context.WorkspaceRoles.Remove(workspaceRole);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckUserAccess(int userId, int workspaceId, AccessRights? accessRight)
        {
            var workspaceUser = await _context
                .WorkspaceUsers
                .Include(wu => wu.WorkspaceRole)
                .Include(wu => wu.Workspace)
                .FirstOrDefaultAsync(wu => wu.UserId == userId && wu.WorkspaceId == workspaceId);

            if (workspaceUser == null)
            {
                return false;
            }

            if (userId == workspaceUser.Workspace.ChiefId)
            {
                return true;
            }

            return accessRight switch
            {
                AccessRights.CanCreate => workspaceUser.WorkspaceRole.CanCreate,
                AccessRights.CanUpdate => workspaceUser.WorkspaceRole.CanUpdate,
                AccessRights.CanDelete => workspaceUser.WorkspaceRole.CanDelete,
                AccessRights.CanManageRoles => workspaceUser.WorkspaceRole.CanManageRoles,
                AccessRights.CanAddUsers => workspaceUser.WorkspaceRole.CanAddUsers,
                null => true,
                _ => throw new ErrorRequestException(500)
            };
        }
    }
}