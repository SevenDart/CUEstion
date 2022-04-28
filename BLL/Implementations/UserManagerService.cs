using System;
using System.Collections.Generic;
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

		public async Task<AuthDto> CreateUser(AuthDto authDto)
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
				SystemRole = "User"
			};

			_context.Users.Add(user);

			await _context.SaveChangesAsync();

			authDto.Id = (await _context.Users.FirstOrDefaultAsync(u => u.Email == authDto.Email)).Id;
			authDto.Role = "User";

			return authDto;
		}

		public async Task<AuthDto> CheckAuthData(AuthDto authDto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == authDto.Email);

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
				authDto.Role = user.SystemRole;
				authDto.Id = user.Id;
			}


			return authDto;
		}

		public async Task UpdateUserInfo(UserDto userDto)
		{
			var user = await _context.Users.FindAsync(userDto.Id);

			if (userDto.Username != null) user.Username = userDto.Username;
			if (userDto.Email != null) user.Email = userDto.Email;

			await _context.SaveChangesAsync();
		}

		public async Task<UserDto> GetUser(int userId)
		{
			var user = await _context.Users.FindAsync(userId);

			return user?.Adapt<UserDto>();
		}

		public async Task DeleteUser(int userId)
		{
			var user = await _context.Users.FindAsync(userId);

			_context.Users.Remove(user);

			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<UserDto>> GetAllUsers()
		{
			var users = await _context.Users.ProjectToType<UserDto>().ToListAsync();
			return users;
		}
	}
}
