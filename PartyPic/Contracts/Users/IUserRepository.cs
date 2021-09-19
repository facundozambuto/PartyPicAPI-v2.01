using PartyPic.DTOs.Users;
using PartyPic.Models.Common;
using PartyPic.Models.Users;

namespace PartyPic.Contracts.Users
{
    public interface IUserRepository
    {
        AllUsersResponse GetAllUsers();
        User GetUserById(int id);
        User CreateUser(User user);
        bool SaveChanges();
        void DeleteUser(int id);
        User UpdateUser(int id, UserUpdateDTO user);
        void PartiallyUpdate(int id, UserUpdateDTO user);
        UserGrid GetAllUsersForGrid(GridRequest gridRequest);
        AllUsersResponse GetAllVenueUsers();
        LoginReadtDTO Login(LoginRequest loginRequest);
    }
}
