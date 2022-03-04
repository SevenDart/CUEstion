namespace DAL.Entities
{
    public class Mark
    {
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int? MarkValue { get; set; }
    }
}