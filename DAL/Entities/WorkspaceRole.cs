using System.Collections.Generic;

namespace DAL.Entities
{
    public class WorkspaceRole
    {
        public int Id { get; set; }
        
        public string Role { get; set; }
        
        public int WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanAddUsers { get; set; }
        
        public List<WorkspaceUser> WorkspaceUsers { get; set; }
    }
}