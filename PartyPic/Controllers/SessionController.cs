using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Logger;
using PartyPic.Contracts.SessionLogs;
using PartyPic.DTOs.SessionLogs;
using PartyPic.DTOs.Users;
using PartyPic.Models.SessionLogs;
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
        private readonly ISessionLogsRepository _sessionLogsRepository;

        public SessionController(IMapper mapper, IConfiguration config, ILoggerManager logger, ISessionLogsRepository sessionLogsRepository) : base(mapper, config, logger)
        {
            _mapper = mapper;
            _logger = logger;
            _sessionLogsRepository = sessionLogsRepository;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<UserApiResponse> Session()
        {
            var user = (User)HttpContext.Items["User"];

            return ExecuteMethod<SessionController, UserApiResponse, User>(() => user);
        }

        [HttpGet]
        [Route("~/api/session/logs")]
        public ActionResult<GetAllSessionLogsApiResponse> GetSessionLogs()
        {
            return ExecuteMethod<SessionController, GetAllSessionLogsApiResponse, AllSessionLogsResponse>(() => _sessionLogsRepository.GetSessionLogs());
        }
    }
}
