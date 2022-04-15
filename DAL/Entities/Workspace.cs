using System.Collections.Generic;

namespace DAL.Entities
{
    public class Workspace
    {
        public int Id { get; set; }
        
        public string Name { get; set; }

        public List<WorkspaceUser> WorkspaceUsers { get; set; } 
    }
}