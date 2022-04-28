using System.Threading.Tasks;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IWorkspaceRoleManagerService
    {
        Task<WorkspaceRoleDto[]> GetAllWorkspaceRoles(int workspaceId);

        Task<WorkspaceRoleDto> GetWorkspaceRoleById(int workspaceRoleId);

        Task<WorkspaceRoleDto> CreateWorkspaceRole(WorkspaceRoleDto workspaceRoleDto);
        
        Task<WorkspaceRoleDto> UpdateWorkspaceRole(int workspaceRoleId, WorkspaceRoleDto workspaceRoleDto);

        Task DeleteWorkspaceRole(int workspaceRoleId);

        Task<bool> CheckUserAccess(int userId, int workspaceId, AccessRights? accessRight = null);
    }
}