using System.Collections;
using System.Collections.Generic;
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
    public class WorkspaceManagerService: IWorkspaceManagerService
    {
        private readonly ApplicationContext _context;
        private readonly IWorkspaceRoleManagerService _workspaceRoleManagerService;
        
        public WorkspaceManagerService(
            ApplicationContext context, 
            IWorkspaceRoleManagerService workspaceRoleManagerService)
        {
            _context = context;
            _workspaceRoleManagerService = workspaceRoleManagerService;
        }
        
        public async Task<IEnumerable<WorkspaceUserDto>> GetUserWorkspaces(int userId)
        {
            var workspaces = await _context
                .WorkspaceUsers
                .Include(wu => wu.Workspace)
                .Include(wu => wu.WorkspaceRole)
                .Where(wu => wu.UserId == userId)
                .ToListAsync();
            
            return workspaces.Adapt<WorkspaceUserDto[]>();
        }

        public async Task<WorkspaceDto> GetWorkspaceById(int workspaceId)
        {
            var workspace = await _context
                .Workspaces
                .Include(w => w.Chief)
                .FirstOrDefaultAsync(w => w.Id == workspaceId);

            return workspace.Adapt<WorkspaceDto>();
        }

        public async Task<WorkspaceUserDto> GetWorkspaceUser(int workspaceId, int userId)
        {
            var workspaceUser = await _context
                .WorkspaceUsers
                .Include(w => w.WorkspaceRole)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.WorkspaceId == workspaceId);

            return workspaceUser.Adapt<WorkspaceUserDto>();
        }

        public async Task<WorkspaceDto> CreateWorkspace(int creatorId, WorkspaceDto workspaceDto)
        {
            Workspace workspace = workspaceDto.Adapt<Workspace>();
            workspace.ChiefId = creatorId;

            await _context.Workspaces.AddAsync(workspace);
            await _context.SaveChangesAsync();
            
            var standardRoleDto = new WorkspaceRoleDto()
            {
                Role = "Admin",
                WorkspaceId = workspace.Id,
                CanCreate = true,
                CanDelete = true,
                CanUpdate = true,
                CanAddUsers = true,
                CanManageRoles = true
            };
            var role = await _workspaceRoleManagerService.CreateWorkspaceRole(standardRoleDto);

            await AddUserToWorkspace(workspace.Id, creatorId, role.Id.Value);

            return workspace.Adapt<WorkspaceDto>();
        }

        public async Task<WorkspaceDto> UpdateWorkspace(int workspaceId, int updaterId, WorkspaceDto workspaceDto)
        {
            var workspace = await _context.Workspaces.FindAsync(workspaceId);

            if (workspace.ChiefId != updaterId)
            {
                throw ErrorRequestException.AccessForbiddenException();
            }

            workspace.Name = (workspace.Name != workspaceDto.Name) ? workspaceDto.Name : workspace.Name;
            workspace.Description = (workspace.Description != workspaceDto.Description) 
                ? workspaceDto.Description 
                : workspace.Description;
            
            if (workspaceDto.ChiefId != updaterId)
            {
                var newChiefUser = await _context
                    .WorkspaceUsers
                    .FirstOrDefaultAsync(wu => wu.WorkspaceId == workspaceId && wu.UserId == workspaceDto.ChiefId);

                if (newChiefUser == null)
                {
                    throw ErrorRequestException.NotFoundException();
                }
                
                workspace.ChiefId = workspaceDto.ChiefId;
            }

            await _context.SaveChangesAsync();

            return workspace.Adapt<WorkspaceDto>();
        }

        public async Task DeleteWorkspace(int workspaceId, int deleterId)
        {
            var workspace = await _context.Workspaces.FindAsync(workspaceId);

            if (workspace.ChiefId != deleterId)
            {
                throw ErrorRequestException.AccessForbiddenException();
            }

            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserToWorkspace(int workspaceId, int newUserId, int workspaceRoleId)
        {
            var workspaceRole = await _context.WorkspaceRoles.FindAsync(workspaceRoleId);

            if (workspaceRole.WorkspaceId != workspaceId)
            {
                throw ErrorRequestException.NotFoundException("Workspace role not found.");
            }
            
            var workspaceUser = new WorkspaceUser()
            {
                WorkspaceId = workspaceId,
                UserId = newUserId,
                WorkspaceRoleId = workspaceRoleId
            };

            await _context.WorkspaceUsers.AddAsync(workspaceUser);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromWorkspace(int workspaceId, int userId)
        {
            var workspaceUser = await _context
                .WorkspaceUsers
                .FirstOrDefaultAsync(wu => wu.UserId == userId && wu.WorkspaceId == workspaceId);

            if (workspaceUser != null)
            {
                _context.WorkspaceUsers.Remove(workspaceUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateWorkspaceUser(int workspaceId, int userId, int workspaceRoleId)
        {
            var workspaceRole = await _context.WorkspaceRoles.FindAsync(workspaceRoleId);

            if (workspaceRole == null || workspaceRole.WorkspaceId != workspaceId)
            {
                throw ErrorRequestException.NotFoundException("Workspace role not found.");
            }
            
            var workspaceUser = await _context
                .WorkspaceUsers
                .FirstOrDefaultAsync(wu => wu.UserId == userId && wu.WorkspaceId == workspaceId);

            if (workspaceUser == null)
            {
                throw ErrorRequestException.NotFoundException("User is not associated to this workspace.");
            }

            workspaceUser.WorkspaceRoleId = workspaceRoleId;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<WorkspaceUserDto>> GetAllWorkspaceUsers(int workspaceId)
        {
            var users = await _context
                .WorkspaceUsers
                .Include(wu => wu.User)
                .Include(wu => wu.WorkspaceRole)
                .Where(wu => wu.WorkspaceId == workspaceId)
                .ToListAsync();

            return users.Adapt<WorkspaceUserDto[]>();
        }
    }
}