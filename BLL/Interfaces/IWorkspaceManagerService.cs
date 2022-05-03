using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.ModelsDTO;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IWorkspaceManagerService
    {
        Task<IEnumerable<WorkspaceUserDto>> GetUserWorkspaces(int userId);

        Task<WorkspaceDto> GetWorkspaceById(int workspaceId);

        Task<WorkspaceUserDto> GetWorkspaceUser(int workspaceId, int userId);

        Task<WorkspaceDto> CreateWorkspace(int creatorId, WorkspaceDto workspaceDto);

        Task<WorkspaceDto> UpdateWorkspace(int workspaceId, int updaterId, WorkspaceDto workspaceDto);

        Task DeleteWorkspace(int workspaceId, int deleterId);

        Task AddUserToWorkspace(int workspaceId, int newUserId, int workspaceRoleId);

        Task RemoveUserFromWorkspace(int workspaceId, int userId);

        Task UpdateWorkspaceUser(int workspaceId, int userId, int workspaceRoleId);

        Task<IEnumerable<WorkspaceUserDto>> GetAllWorkspaceUsers(int workspaceId);
    }
}