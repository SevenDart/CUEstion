using CUEstion.DAL.Entities;

namespace CUEstion.BLL.ModelsDTO
{
	public class UserDTO
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public int Rate { get; set; }

		public UserDTO()
		{

		}

		public UserDTO(User user)
		{
			Id = user.Id;
			Email = user.Email;
			Username = user.Username;
			Rate = user.Rate;
		}
	}
}
