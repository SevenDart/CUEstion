﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Interfaces;
using BLL.ModelsDTO;
using DAL.EF;
using DAL.Entities;
using Mapster;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementations
{
	public class UserManagerService : IUserManagerService
	{
		private readonly ApplicationContext _context;

		public UserManagerService(ApplicationContext context)
		{
			_context = context;
		}


		public AuthDto CreateUser(AuthDto authDto)
		{
			byte[] salt = new byte[128 / 8];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}
			string saltHash = Encoding.Unicode.GetString(salt);
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: authDto.Password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 10000,
				numBytesRequested: 256 / 8
			));


			var user = new User()
			{
				Email = authDto.Email,
				Username = new Regex("@.+").Replace(authDto.Email, ""),
				Password = hashed,
				Salt = saltHash,
				Role = "User"
			};

			_context.Users.Add(user);

			_context.SaveChanges();

			authDto.Id = _context.Users.FirstOrDefault(u => u.Email == authDto.Email).Id;
			authDto.Role = "User";

			return authDto;
		}

		public AuthDto CheckAuthData(AuthDto authDto)
		{
			var user = _context.Users.FirstOrDefault(u => u.Email == authDto.Email);

			if (user == null)
			{
				return null;
			}

			var salt = Encoding.Unicode.GetBytes(user.Salt);
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: authDto.Password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 10000,
				numBytesRequested: 256 / 8
			));

			if (user.Password != hashed)
				authDto = null;
			else
			{
				authDto.Role = user.Role;
				authDto.Id = user.Id;
			}


			return authDto;
		}


		public void UpdateUserInfo(UserDto userDto)
		{
			var user = _context.Users.Find(userDto.Id);

			if (userDto.Username != null) user.Username = userDto.Username;
			if (userDto.Email != null) user.Email = userDto.Email;

			_context.SaveChanges();
		}


		public UserDto GetUser(int userId)
		{
			var user = _context.Users.Find(userId);

			return user != null 
				? new UserDto(user) 
				: null;
		}

		public void DeleteUser(int userId)
		{
			var user = _context.Users.Find(userId);

			_context.Users.Remove(user);

			_context.SaveChanges();
		}

		public IEnumerable<UserDto> GetAllUsers()
		{
			var users = _context.Users.ToList();
			var list = new List<UserDto>();
			foreach (var user in users)
			{
				list.Add(new UserDto(user));
			}
			return list;
		}

		public async Task<IEnumerable<QuestionDto>> GetUsersQuestions(int userId)
		{
			var questions = await _context
				.Questions
				.Include(q => q.Tags)
				.Where(q => q.UserId == userId)
				.ProjectToType<QuestionDto>()
				.ToListAsync();

			return questions;
		}

		public async Task<IEnumerable<QuestionDto>> GetFollowedQuestions(int userId)
		{
			var questions = await _context
				.FollowedQuestions
				.Include(fq => fq.Question)
				.ThenInclude(q => q.Tags)
				.Where(fq => fq.UserId == userId)
				.Select(fq => fq.Question)
				.ProjectToType<QuestionDto>()
				.ToListAsync();
			

			return questions;
		}
	}
}
