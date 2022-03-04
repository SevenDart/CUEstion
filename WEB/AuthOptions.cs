using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WEB
{
	public class AuthOptions
	{
        public const string ISSUER = "CUEstionAuthServer";
        public const string AUDIENCE = "CUEstionAuthClient";
        const string KEY = "thisKeyisnotsupersecret";
        public const int LIFETIME = 6;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
