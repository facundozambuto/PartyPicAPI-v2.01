using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Roles;
using PartyPic.DTOs.Roles;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Roles;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PartyPic.Models.Common;
using PartyPic.Helpers;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/roles")]
    [ApiController]
    public class RoleController : PartyPicControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public RoleController(IRoleRepository roleRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllRoles()
        {
            return ExecuteMethod<RoleController, GetAllRolesApiResponse, AllRolesResponse>(() => _roleRepository.GetAllRoles());
        }

        [HttpGet]
        [Route("~/api/roles/grid")]
        [Authorize]
        [AuthorizeRole(1)]
        public ActionResult<RoleGrid> GetAllRolesForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<RoleController, GridRoleApiResponse, RoleGrid>(() => _roleRepository.GetAllRolesForGrid(gridRequest));
        }

        [HttpGet("{id}", Name = "GetRoleById")]
        [Authorize]
        public ActionResult<Role> GetRoleById(int id)
        {
            return ExecuteMethod<RoleController, RoleApiResponse, Role>(() => _roleRepository.GetRoleById(id));
        }

        [HttpPost]
        [Authorize]
        [AuthorizeRole(1)]
        public ActionResult<Role> CreateRole(RoleCreateDTO roleCreateDTO)
        {
            var roleModel = _mapper.Map<Role>(roleCreateDTO);

            return ExecuteMethod<RoleController, RoleApiResponse, Role>(() => _roleRepository.CreateRole(roleModel));
        }

        [HttpPut("{id}")]
        [Authorize]
        [AuthorizeRole(1)]
        public ActionResult UpdateRole(int id, RoleUpdateDTO roleUpdateDto)
        {
            return ExecuteMethod<RoleController, RoleApiResponse, Role>(() => _roleRepository.UpdateRole(id, roleUpdateDto));
        }

        [HttpPatch("{id}")]
        [Authorize]
        [AuthorizeRole(1)]
        public ActionResult PartialRoleUpdate(int id, JsonPatchDocument<RoleUpdateDTO> patchDoc)
        {
            var roleModelFromRepo = _roleRepository.GetRoleById(id);
            if (roleModelFromRepo == null)
            {
                return ExecuteMethod<RoleController>(() => new NotRoleFoundException());
            }

            var roleToPatch = _mapper.Map<RoleUpdateDTO>(roleModelFromRepo);
            patchDoc.ApplyTo(roleToPatch);

            if (!TryValidateModel(roleToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(roleToPatch, roleModelFromRepo);

            return ExecuteMethod<RoleController>(() => _roleRepository.PartiallyUpdate(id, roleToPatch));
        }

        [HttpDelete("{id}")]
        [Authorize]
        [AuthorizeRole(1)]
        public ActionResult DeleteRole(int id)
        {
            return ExecuteMethod<RoleController>(() => _roleRepository.DeleteRole(id));
        }
    }
}