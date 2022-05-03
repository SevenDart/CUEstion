using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class Workspace
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [ForeignKey("Chief")]
        public int ChiefId { get; set; }
        public User Chief { get; set; }

        public List<WorkspaceUser> WorkspaceUsers { get; set; } 
        
        public List<Question> Questions { get; set; }
    }
}