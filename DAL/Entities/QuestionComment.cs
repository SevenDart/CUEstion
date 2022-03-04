namespace DAL.Entities
{
    public class QuestionComment: Comment
    {
        public int? QuestionId { get; set; }
        public Question Question { get; set; }
    }
}