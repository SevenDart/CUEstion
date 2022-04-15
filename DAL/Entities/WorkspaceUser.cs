namespace DAL.Entities
{
    public class WorkspaceUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        
        public int WorkspaceRoleId { get; set; }
        public WorkspaceRole WorkspaceRole { get; set; }
    }
}