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
        private readonly Contracts.Logger.ILoggerManager _logger;

        public UserController(IUserRepository userRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllUsers()
        {
            return ExecuteMethod<UserController, GetAllUsersApiResponse, AllUsersResponse>(() =>_userRepository.GetAllUsers());
        }

        [Authorize]
        [HttpGet]
        [Route("~/api/users/venueUsers")]
        public ActionResult GetAllVenueUsers()
        {
            return ExecuteMethod<UserController, GetAllUsersApiResponse, AllUsersResponse>(() => _userRepository.GetAllVenueUsers());
        }

        [Authorize]
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

        [Authorize]
        [HttpGet("{userId}", Name = "GetUserById")]
        public ActionResult<User> GetUserById(int userId)
        {
           return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.GetUserById(userId));
        }

        [Authorize]
        [HttpPost]
        public ActionResult<User> CreateUser(UserCreateDTO userCreateDTO)
        {
            var userModel = _mapper.Map<User>(userCreateDTO);

            return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.CreateUser(userModel));
        }

        [Authorize]
        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, UserUpdateDTO userUpdateDto)
        {
            return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.UpdateUser(id, userUpdateDto));
        }

        [Authorize]
        [HttpPatch("{userId}")]
        public ActionResult PartialUserUpdate(int userId, JsonPatchDocument<UserUpdateDTO> patchDoc)
        {
            var userModelFromRepo = _userRepository.GetUserById(userId);
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

            return ExecuteMethod<UserController>(() => _userRepository.PartiallyUpdate(userId, userToPatch));
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(int userId)
        {
            return ExecuteMethod<UserController>(() => _userRepository.DeleteUser(userId));
        }

        [Authorize]
        [HttpPut("currentUser/{userId}", Name = "UpdateCurrentUser")]
        [Route("~/api/users/")]
        public ActionResult UpdateCurrentUser(int userId, UserUpdateDTO userUpdateDto)
        {
            return ExecuteMethod<UserController, UserApiResponse, User>(() => _userRepository.UpdateCurrentUser(userId, userUpdateDto));
        }

        [Authorize]
        [HttpPost("passwordUpdate/", Name = "ChangeUserPassword")]
        [Route("~/api/users/")]
        public ActionResult ChangeUserPassword([FromBody] ChangePasswordRequest changePasswordRequest)
        {
            return ExecuteMethod<UserController>(() => _userRepository.ChangeUserPassword(changePasswordRequest));
        }
    }
}
