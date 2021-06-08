using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using CUEstion.DAL.EF;
using CUEstion.BLL.ModelsDTO;
using CUEstion.DAL.Entities;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text.RegularExpressions;

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
				authDto.Role = user.Role;


			return authDto;
		}


		public UserDTO GetUser(int userId)
		{
			using var context = new ApplicationContext();

			var user = context.Users.Find(userId);

			return new UserDTO(user);
		}
	}
}
