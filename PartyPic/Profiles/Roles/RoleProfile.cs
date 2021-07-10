using AutoMapper;
using PartyPic.DTOs.Roles;
using PartyPic.Models.Roles;

namespace PartyPic.Profiles.Roles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<Role, RoleReadDTO>();
            CreateMap<RoleCreateDTO, Role>();
            CreateMap<RoleGrid, RoleReadDTOGrid>();
            CreateMap<RoleReadDTOGrid, RoleGrid>();
            CreateMap<RoleUpdateDTO, Role>();
            CreateMap<Role, RoleUpdateDTO>();
            CreateMap<AllRolesResponse, GetAllRolesApiResponse>();
            CreateMap<GetAllRolesApiResponse, AllRolesResponse>();
            CreateMap<AllRolesResponse, GetAllRolesApiResponse>();
            CreateMap<GetAllRolesApiResponse, AllRolesResponse>();
            CreateMap<RoleGrid, GridRoleApiResponse>();
            CreateMap<GridRoleApiResponse, RoleGrid>();
            CreateMap<RoleApiResponse, Role>();
            CreateMap<Role, RoleApiResponse>();
        }
    }
}
