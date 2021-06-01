using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CUEstion.DAL.Entities;
using Microsoft.Extensions.Configuration;

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
			modelBuilder.Entity<Answer>().HasOne(a => a.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Question>().HasOne(q => q.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Comment>().HasOne(c => c.User).WithMany().OnDelete(DeleteBehavior.SetNull);
			modelBuilder.Entity<Comment>().HasOne(c => c.Answer).WithMany(a => a.Comments).HasForeignKey(c => c.AnswerId).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Comment>().HasOne(c => c.Question).WithMany(q => q.Comments).HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.NoAction);
		}

	}
}
