using DAL.Entities;

namespace BLL.ModelsDTO
{
	public class UserDto
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public int Rate { get; set; }
	}
}
