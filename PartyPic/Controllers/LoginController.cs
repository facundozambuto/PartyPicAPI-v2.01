using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Logger;
using PartyPic.Contracts.Users;
using PartyPic.DTOs.Users;
using PartyPic.Models.Users;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/login")]
    [ApiController]
    public class LoginController : PartyPicControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public LoginController(IUserRepository userRepository, IMapper mapper, IConfiguration config, ILoggerManager logger) : base(mapper, config, logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<LoginApiResponse> LoginUser([FromQuery] string email, [FromQuery] string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            return ExecuteMethod<LoginController, LoginApiResponse, LoginReadtDTO>(() => _userRepository.Login(loginRequest));
        }
    }
}
