using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using CUEstion.DAL.Entities;
using Microsoft.Extensions.Configuration;

namespace CUEstion.DAL.EF
{
	class ApplicationContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Answer> Answers { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<FollowedQuestions> FollowedQuestions { get; set; }

		public ApplicationContext() : base()
		{

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(@"Server=DESKTOP-KH4PKN3;Database=CUEstionDB;Trusted_Connection=True;");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<FollowedQuestions>().HasKey(fq => new { fq.UserId, fq.QuestionId});
			modelBuilder.Entity<Answer>().Property("CreatorId").IsRequired();
			modelBuilder.Entity<Answer>().Property("QuestionId").IsRequired();
			modelBuilder.Entity<Comment>().Property("CreatorId").IsRequired();
			modelBuilder.Entity<Question>().Property("CreatorId").IsRequired();
			modelBuilder.Entity<Answer>().HasOne(a => a.Question).WithMany(q => q.Answers).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<Question>().HasMany(q => q.FollowedQuestions).WithOne(fq => fq.Question).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<User>().HasMany(u => u.FollowedQuestions).WithOne(fq => fq.User).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<FollowedQuestions>().HasOne(fq => fq.User).WithMany(u => u.FollowedQuestions).OnDelete(DeleteBehavior.NoAction);
			modelBuilder.Entity<FollowedQuestions>().HasOne(fq => fq.Question).WithMany(q => q.FollowedQuestions).OnDelete(DeleteBehavior.NoAction);
		}

	}
}
