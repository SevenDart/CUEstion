using Microsoft.EntityFrameworkCore;
using DAL.Entities;


namespace DAL.EF
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<QuestionComment> QuestionComments { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<AnswerComment> AnswerComments { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<FollowedQuestion> FollowedQuestions { get; set; }
		
		public DbSet<QuestionMark> QuestionMarks { get; set; }
		public DbSet<AnswerMark> AnswerMarks { get; set; }
		public DbSet<CommentMark> CommentMarks { get; set; }

		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AnswerComment>().ToTable("AnswerComments");
			modelBuilder.Entity<QuestionComment>().ToTable("QuestionComments");

			modelBuilder.Entity<QuestionMark>().HasKey(qm => new { qm.UserId, qm.QuestionId });
			modelBuilder.Entity<AnswerMark>().HasKey(am => new { am.UserId, am.AnswerId });
			modelBuilder.Entity<CommentMark>().HasKey(cm => new { cm.UserId, cm.CommentId });
			
			modelBuilder.Entity<FollowedQuestion>().HasKey(fq => new { fq.UserId, fq.QuestionId});
			
			modelBuilder.Entity<Question>().HasOne(q => q.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Answer>().HasOne(a => a.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany().OnDelete(DeleteBehavior.SetNull);

			modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
		}

	}
}
