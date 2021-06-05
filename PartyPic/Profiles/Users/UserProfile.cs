using AutoMapper;
using PartyPic.DTOs.Users;
using PartyPic.Models.Users;

namespace PartyPic.Profiles.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDTO>();
            CreateMap<UserCreateDTO, User>();
            CreateMap<UserGrid, UserReadDTOGrid>();
            CreateMap<UserReadDTOGrid, UserGrid>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserUpdateDTO>();
            CreateMap<AllUsersResponse, GetAllUsersApiResponse>();
            CreateMap<GetAllUsersApiResponse, AllUsersResponse>();
            CreateMap<AllUsersResponse, GetAllUsersApiResponse>();
            CreateMap<GetAllUsersApiResponse, AllUsersResponse>();
            CreateMap<UserGrid, GridUserApiResponse>();
            CreateMap<GridUserApiResponse, UserGrid>();
            CreateMap<UserApiResponse, User>();
            CreateMap<User, UserApiResponse>();
        }
    }
}
