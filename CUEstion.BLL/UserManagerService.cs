using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CUEstion.DAL.EF;
using CUEstion.BLL.ModelsDTO;
using CUEstion.DAL.Entities;

namespace CUEstion.BLL
{
	public class UserManagerService
	{
		public AuthDTO CreateUser(AuthDTO authDto)
		{
			using var context = new ApplicationContext();

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

			context.Users.Add(user);

			context.SaveChanges();

			authDto.Id = context.Users.FirstOrDefault(u => u.Email == authDto.Email).Id;
			authDto.Role = "User";

			return authDto;
		}

		public AuthDTO CheckAuthData(AuthDTO authDto)
		{
			using var context = new ApplicationContext();

			var user = context.Users.FirstOrDefault(u => u.Email == authDto.Email);

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
			using var context = new ApplicationContext();

			var user = context.Users.Find(userDto.Id);

			if (userDto.Username != null) user.Username = userDto.Username;
			if (userDto.Email != null) user.Email = userDto.Email;

			context.SaveChanges();
		}


		public UserDTO GetUser(int userId)
		{
			using var context = new ApplicationContext();

			var user = context.Users.Find(userId);

			return user != null 
				? new UserDTO(user) 
				: null;
		}

		public void DeleteUser(int userId)
		{
			using var context = new ApplicationContext();

			var user = context.Users.Find(userId);

			context.Users.Remove(user);

			context.SaveChanges();
		}

		public IEnumerable<UserDTO> GetAllUsers()
		{
			using var context = new ApplicationContext();

			var users = context.Users.ToList();
			var list = new List<UserDTO>();
			foreach (var user in users)
			{
				list.Add(new UserDTO(user));
			}
			return list;
		}

		public IEnumerable<QuestionDTO> GetUsersQuestions(int userId)
		{
			using var context = new ApplicationContext();

			var questions = context.Questions.Where(q => q.UserId == userId);
			var list = new List<QuestionDTO>();
			foreach (var question in questions)
			{
				list.Add(new QuestionDTO(question));
			}

			return list;
		}
	}
}
