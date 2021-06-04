using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUEstion.BLL.ModelsDTO
{
	public class AuthDTO
	{
		public int Id { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public string Role { get; set; }
	}
}
