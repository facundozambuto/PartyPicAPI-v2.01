using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Logger;
using PartyPic.DTOs.Users;
using PartyPic.Models.Users;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/session")]
    [ApiController]
    public class SessionController : PartyPicControllerBase
    {
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public SessionController(IMapper mapper, IConfiguration config, ILoggerManager logger) : base(mapper, config, logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<UserApiResponse> Session()
        {
            var user = (User)HttpContext.Items["User"];

            return ExecuteMethod<SessionController, UserApiResponse, User>(() => user);
        }
    }
}
