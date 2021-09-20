using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Logger;
using PartyPic.Contracts.Users;
using PartyPic.DTOs.Users;
using PartyPic.Models.Users;
using RestSharp;
using System;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
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
        [Route("api/login")]
        public ActionResult<LoginApiResponse> LoginUser([FromQuery] string email, [FromQuery] string password)
        {
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            return ExecuteMethod<LoginController, LoginApiResponse, LoginReadtDTO>(() => _userRepository.Login(loginRequest));
        }

        [HttpDelete]
        [Authorize]
        [Route("api/login")]
        public ActionResult LogOut()
        {
            var currentUserCookie = HttpContext.Request.Cookies["AppSessionId"];

            HttpContext.Response.Cookies.Delete("AppSessionId");

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-10)
            };

            HttpContext.Response.Cookies.Append("AppSessionId", "", cookieOptions);

            HttpContext.Items["User"] = null;

            return ExecuteMethod<LoginController>(() => Ok());
        }

        [HttpGet]
        [Route("api/passwordRecover")]
        public ActionResult RecoverPassword([FromQuery] string email)
        {
            return ExecuteMethod<LoginController>(() => _userRepository.RecoverPassword(email));
        }
    }
}
