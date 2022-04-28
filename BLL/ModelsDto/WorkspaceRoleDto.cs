namespace BLL.ModelsDTO
{
    public class WorkspaceRoleDto
    {
        public int? Id { get; set; }
        
        public string Role { get; set; }
        
        public int WorkspaceId { get; set; }

        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanManageRoles { get; set; }
        public bool CanAddUsers { get; set; }
    }
}