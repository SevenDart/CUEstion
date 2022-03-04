using System.Collections.Generic;
using BLL.ModelsDTO;

namespace BLL.Interfaces
{
    public interface IUserManagerService
    {
        public AuthDTO CreateUser(AuthDTO authDto);
        public AuthDTO CheckAuthData(AuthDTO authDto);
        public void UpdateUserInfo(UserDTO userDto);
        public UserDTO GetUser(int userId);
        public void DeleteUser(int userId);
        public IEnumerable<UserDTO> GetAllUsers();
        public IEnumerable<QuestionDTO> GetUsersQuestions(int userId);
        public IEnumerable<QuestionDTO> GetFollowedQuestions(int userId);
    }
}