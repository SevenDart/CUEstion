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
				Password = hashed,
				Salt = saltHash,
				Role = "User"
			};

			context.Users.Add(user);

			context.SaveChanges();

			authDto.Id = context.Users.FirstOrDefault(u => u.Email == authDto.Email).Id;

			return authDto;
		}

		public AuthDTO CheckAuthData(AuthDTO authDto)
		{
			using var context = new ApplicationContext();

			var user = context.Users.FirstOrDefault(u => u.Email == authDto.Email && u.Password == authDto.Password);

			if (user != null)
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
