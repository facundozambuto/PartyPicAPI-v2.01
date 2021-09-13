using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PartyPic.Contracts.Users;
using PartyPic.DTOs.Users;
using PartyPic.Models.Users;
using PartyPic.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.JsonPatch;
using PartyPic.Models.Exceptions;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/users")]
    [ApiController]
    public class UserController : PartyPicControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper, IConfiguration config) : base(mapper, config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            return ExecuteMethod<UserController, GetAllUsersApiResponse, AllUsersResponse>(() =>_userRepository.GetAllUsers());
        }

        [HttpGet]
        [Route("~/api/users/venueUsers")]
        public ActionResult GetAllVenueUsers()
        {
            return ExecuteMethod<UserController, GetAllUsersApiResponse, AllUsersResponse>(() => _userRepository.GetAllVenueUsers());
        }

        [HttpGet]
        [Route("~/api/users/grid")]
        public ActionResult<UserGrid> GetAllUsersForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<UserController, GridUserApiResponse, UserGrid>(() => _userRepository.GetAllUsersForGrid(gridRequest));
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public ActionResult<User> GetUserById(int id)
        {
           return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.GetUserById(id));
        }

        [HttpPost]
        public ActionResult<User> CreateUser(UserCreateDTO userCreateDTO)
        {
            var userModel = _mapper.Map<User>(userCreateDTO);

            return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.CreateUser(userModel));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UserUpdateDTO userUpdateDto)
        {
            return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.UpdateUser(id, userUpdateDto));
        }

        [HttpPatch("{id}")]
        public ActionResult PartialUserUpdate(int id, JsonPatchDocument<UserUpdateDTO> patchDoc)
        {
            var userModelFromRepo = _userRepository.GetUserById(id);
            if (userModelFromRepo == null)
            {
                return ExecuteMethod<UserController>(() => new NotUserFoundException());
            }

            var userToPatch = _mapper.Map<UserUpdateDTO>(userModelFromRepo);
            patchDoc.ApplyTo(userToPatch);

            if (!TryValidateModel(userToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(userToPatch, userModelFromRepo);

            return ExecuteMethod<UserController>(() => _userRepository.PartiallyUpdate(id, userToPatch));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            return ExecuteMethod<UserController>(() => _userRepository.DeleteUser(id));
        }
    }
}
