using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace WEB
{
	public static class Tools
	{
		public static string CreateToken(string email, int id, string role)
		{
			var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Email, email),
					new Claim(ClaimTypes.Sid, id.ToString()),
					new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
				};

			var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimTypes.Email, ClaimTypes.Role);

			var now = DateTime.UtcNow;
			var jwt = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: AuthOptions.AUDIENCE,
				notBefore: now,
				claims: claimsIdentity.Claims,
				expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFETIME)),
				signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
				SecurityAlgorithms.HmacSha256)
			);

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return encodedJwt;
		}
	}
}
