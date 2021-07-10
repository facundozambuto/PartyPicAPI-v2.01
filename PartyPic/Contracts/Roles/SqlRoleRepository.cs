using AutoMapper;
using PartyPic.DTOs.Roles;
using PartyPic.Helpers;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.Roles
{
    public class SqlRoleRepository : IRoleRepository
    {
        private readonly RoleContext _roleContext;
        private readonly IMapper _mapper;

        public SqlRoleRepository(RoleContext roleContext, IMapper mapper)
        {
            _roleContext = roleContext;
            _mapper = mapper;
        }

        public Role CreateRole(Role role)
        {
            this.ThrowExceptionIfArgumentIsNull(role);
            this.ThrowExceptionIfPropertyAlreadyExists(role, true, 0);

            role.CreatedDatetime = DateTime.Now;

            _roleContext.Roles.Add(role);

            this.SaveChanges();

            var addedRole = _roleContext.Roles.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedRole;
        }

        public void DeleteRole(int id)
        {
            var role = this.GetRoleById(id);

            if (role == null)
            {
                throw new NotRoleFoundException();
            }

            _roleContext.Roles.Remove(role);

            this.SaveChanges();
        }

        public AllRolesResponse GetAllRoles()
        {
            return new AllRolesResponse
            {
                Roles = _roleContext.Roles.ToList()
            };
        }

        public RoleGrid GetAllRolesForGrid(GridRequest gridRequest)
        {
            var rolesRows = new List<Role>();

            rolesRows = _roleContext.Roles.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                rolesRows = _roleContext.Roles.Where(cat => cat.Description.Contains(gridRequest.SearchPhrase)).ToList();
            }

            if (gridRequest.RowCount != -1 && _roleContext.Roles.Count() > gridRequest.RowCount && gridRequest.Current > 0 && rolesRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((rolesRows.Count % gridRequest.RowCount) != 0 && (rolesRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = rolesRows.Count % gridRequest.RowCount;
                }

                rolesRows = rolesRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                rolesRows = rolesRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    rolesRows.Reverse();
                }
            }

            var rolesGrid = new RoleGrid
            {
                Rows = rolesRows,
                Total = _roleContext.Roles.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return rolesGrid;
        }

        public Role GetRoleById(int id)
        {
            var role = _roleContext.Roles.FirstOrDefault(rol => rol.RoleId == id);

            if (role == null)
            {
                throw new NotCategoryFoundException();
            }

            return role;
        }

        public void PartiallyUpdate(int id, RoleUpdateDTO role)
        {
            this.UpdateRole(id, role);

            this.SaveChanges();
        }

        public bool SaveChanges()
        {
            return (_roleContext.SaveChanges() >= 0);
        }

        public Role UpdateRole(int id, RoleUpdateDTO updateRoleDTO)
        {
            var role = _mapper.Map<Role>(updateRoleDTO);

            var retrievedRole = this.GetRoleById(id);

            if (retrievedRole == null)
            {
                throw new NotCategoryFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(role);
            this.ThrowExceptionIfPropertyIsIncorrect(role, false, id);

            _mapper.Map(updateRoleDTO, retrievedRole);

            _roleContext.Roles.Update(retrievedRole);

            this.SaveChanges();

            return this.GetRoleById(id);
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Role role, bool isNew, int id)
        {
            if (_roleContext.Roles.ToList().Any(rol => rol.Description == role.Description))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(Role role, bool isNew, int id)
        {
            if (!isNew)
            {
                if (role.CreatedDatetime != _roleContext.Roles.FirstOrDefault(r => r.RoleId == id).CreatedDatetime)
                {
                    throw new PropertyIncorrectException();
                }
            }

            if (_roleContext.Roles.ToList().Any(rol => rol.Description == role.Description))
            {
                throw new PropertyIncorrectException();
            }
        }

        private void ThrowExceptionIfArgumentIsNull(Role role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            if (string.IsNullOrEmpty(role.Description))
            {
                throw new ArgumentNullException(nameof(role.Description));
            }
        }
    }
}
