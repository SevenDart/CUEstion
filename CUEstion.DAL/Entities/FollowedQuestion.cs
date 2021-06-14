namespace CUEstion.DAL.Entities
{
	public class FollowedQuestion
	{
		public int UserId { get; set; }
		public User User { get; set; }

		public int QuestionId { get; set; }
		public Question Question { get; set; }
	}
}
