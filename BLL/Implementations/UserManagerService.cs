﻿using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using BLL.Interfaces;
using BLL.ModelsDTO;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using DAL.EF;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BLL
{
	public class UserManagerService : IUserManagerService
	{
		private readonly ApplicationContext _context;

		public UserManagerService(ApplicationContext context)
		{
			_context = context;
		}


		public AuthDTO CreateUser(AuthDTO authDto)
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

		public AuthDTO CheckAuthData(AuthDTO authDto)
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


		public void UpdateUserInfo(UserDTO userDto)
		{
			var user = _context.Users.Find(userDto.Id);

			if (userDto.Username != null) user.Username = userDto.Username;
			if (userDto.Email != null) user.Email = userDto.Email;

			_context.SaveChanges();
		}


		public UserDTO GetUser(int userId)
		{
			var user = _context.Users.Find(userId);

			return user != null 
				? new UserDTO(user) 
				: null;
		}

		public void DeleteUser(int userId)
		{
			var user = _context.Users.Find(userId);

			_context.Users.Remove(user);

			_context.SaveChanges();
		}

		public IEnumerable<UserDTO> GetAllUsers()
		{
			var users = _context.Users.ToList();
			var list = new List<UserDTO>();
			foreach (var user in users)
			{
				list.Add(new UserDTO(user));
			}
			return list;
		}

		public IEnumerable<QuestionDTO> GetUsersQuestions(int userId)
		{
			var questions = _context.Questions.Include(q => q.Tags).Where(q => q.UserId == userId);
			var list = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				list.Add(new QuestionDTO(question));
			}

			return list;
		}

		public IEnumerable<QuestionDTO> GetFollowedQuestions(int userId)
		{
			var questions = _context.FollowedQuestions.Include(fq => fq.Question).ThenInclude(q => q.Tags).Where(fq => fq.UserId == userId).Select(fq => fq.Question);

			var list = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				list.Add(new QuestionDTO(question));
			}

			return list;
		}
	}
}