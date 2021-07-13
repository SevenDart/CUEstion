using Microsoft.EntityFrameworkCore;
using CUEstion.DAL.Entities;


namespace CUEstion.DAL.EF
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<FollowedQuestion> FollowedQuestions { get; set; }

		public DbSet<QuestionMark> QuestionMarks { get; set; }
		public DbSet<AnswerMark> AnswerMarks { get; set; }
		public DbSet<CommentMark> CommentMarks { get; set; }

		public ApplicationContext() : base()
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=DESKTOP-KH4PKN3;Database=CUEstionDB;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<FollowedQuestion>().HasKey(fq => new { fq.UserId, fq.QuestionId});
			modelBuilder.Entity<QuestionMark>().HasKey(qm => new { qm.UserId, qm.QuestionId });
			modelBuilder.Entity<AnswerMark>().HasKey(am => new { am.UserId, am.AnswerId });
			modelBuilder.Entity<CommentMark>().HasKey(cm => new { cm.UserId, cm.CommentId });
			modelBuilder.Entity<Answer>().HasOne(a => a.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Question>().HasOne(q => q.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Comment>().HasOne(c => c.Answer).WithMany(a => a.Comments).HasForeignKey(c => c.AnswerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Comment>().HasOne(c => c.Question).WithMany(q => q.Comments).HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
		}

	}
}
