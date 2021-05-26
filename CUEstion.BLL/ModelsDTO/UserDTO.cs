using CUEstion.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CUEstion.BLL.ModelsDTO
{
	public class UserDTO
	{
		public int Id { get; set; }

		public string Username { get; set; }

		public int Rate { get; set; }

		public UserDTO()
		{

		}


		public UserDTO(User user)
		{
			Id = user.Id; 
			Username = user.Username;
		}
	}
}
