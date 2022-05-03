using System.Collections.Generic;
using DAL.Entities;

namespace BLL.ModelsDTO
{
    public class WorkspaceDto
    {
        public int? Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int ChiefId { get; set; }
        
        public UserDto Chief { get; set; }
    }
}