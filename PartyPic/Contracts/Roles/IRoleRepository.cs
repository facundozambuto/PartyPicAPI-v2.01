using PartyPic.DTOs.Roles;
using PartyPic.Models.Common;
using PartyPic.Models.Roles;

namespace PartyPic.Contracts.Roles
{
    public interface IRoleRepository
    {
        AllRolesResponse GetAllRoles();
        Role GetRoleById(int id);
        Role CreateRole(Role role);
        bool SaveChanges();
        void DeleteRole(int id);
        Role UpdateRole(int id, RoleUpdateDTO updateRoleDTO);
        void PartiallyUpdate(int id, RoleUpdateDTO updateRoleDTO);
        RoleGrid GetAllRolesForGrid(GridRequest gridRequest);
    }
}
