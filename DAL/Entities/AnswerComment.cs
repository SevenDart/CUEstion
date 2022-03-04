namespace DAL.Entities
{
    public class AnswerComment : Comment
    {
        public int? AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}