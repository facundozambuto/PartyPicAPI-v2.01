using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Reports;
using PartyPic.DTOs;
using PartyPic.Models.Reports;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/reports")]
    [ApiController]
    public class ReportsController : PartyPicControllerBase
    {
        private readonly IReportsRepository _reportsRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public ReportsController(IReportsRepository reportsRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _reportsRepository = reportsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetReports()
        {
            return ExecuteMethod<CategoryController, GetReportsApiResponse, ReportsResponse>(() => _reportsRepository.GetReports());
        }
    }
}
