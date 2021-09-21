using Microsoft.AspNetCore.Mvc;
using PartyPic.DTOs.Users;
using PartyPic.Models.Common;
using PartyPic.Models.Users;

namespace PartyPic.Contracts.Users
{
    public interface IUserRepository
    {
        AllUsersResponse GetAllUsers();
        User GetUserById(int userId);
        User CreateUser(User user);
        bool SaveChanges();
        void DeleteUser(int userId);
        User UpdateUser(int userId, UserUpdateDTO user);
        void PartiallyUpdate(int userId, UserUpdateDTO user);
        UserGrid GetAllUsersForGrid(GridRequest gridRequest);
        AllUsersResponse GetAllVenueUsers();
        LoginReadtDTO Login(LoginRequest loginRequest);
        void RecoverPassword(string email);
        User UpdateCurrentUser(int userId, UserUpdateDTO user);
        void ChangeUserPassword(ChangePasswordRequest changePasswordRequest);
    }
}
