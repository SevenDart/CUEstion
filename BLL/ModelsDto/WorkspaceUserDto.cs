namespace BLL.ModelsDTO
{
    public class WorkspaceUserDto
    {
        public int UserId { get; set; }
        public UserDto User { get; set; }
        
        public int WorkspaceId { get; set; }
        public WorkspaceDto Workspace { get; set; }
        
        public int WorkspaceRoleId { get; set; }
        public WorkspaceRoleDto WorkspaceRole { get; set; }
    }
}