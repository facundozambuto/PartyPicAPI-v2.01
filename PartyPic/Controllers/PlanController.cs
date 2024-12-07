using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PartyPic.Contracts.Plans;
using PartyPic.DTOs.Plans;
using PartyPic.Models.Plans;
using PartyPic.Models.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.JsonPatch;
using PartyPic.Models.Exceptions;
using System.Collections.Generic;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/plans")]
    [ApiController]
    public class PlanController : PartyPicControllerBase
    {
        private readonly IPlanRepository _planRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public PlanController(IPlanRepository planRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _planRepository = planRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllPlans()
        {
            return ExecuteMethod<PlanController, GetAllPlansApiResponse, AllPlansResponse>(() =>_planRepository.GetAllPlans());
        }

        [Authorize]
        [HttpGet]
        [Route("~/api/plans/grid")]
        public ActionResult<PlanGrid> GetAllPlansForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<PlanController, GridPlanApiResponse, PlanGrid>(() => _planRepository.GetAllPlansForGrid(gridRequest));
        }

        [Authorize]
        [HttpGet("{planId}", Name = "GetPlanById")]
        public ActionResult<Plan> GetPlanById(int planId)
        {
           return ExecuteMethod<PlanController, PlanApiResponse, PlanReadDTO>(() => _planRepository.GetPlanById(planId));
        }

        [Authorize]
        [HttpGet]
        [Route("~/api/plans/pricehistory")]
        public ActionResult<IEnumerable<PlanPriceHistoryDTO>> GetPricesByPlanId([FromQuery] int planId)
        {
            var priceHistory = _planRepository.GetPlanPriceHistory(planId);

            return Ok(priceHistory);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Plan> CreatePlan(PlanCreateDTO planCreateDTO)
        {
            return ExecuteMethod<PlanController, PlanApiResponse, PlanReadDTO>(() => _planRepository.CreatePlan(planCreateDTO));
        }

        [Authorize]
        [HttpPut("{id}")]
        public ActionResult UpdatePlan(int id, PlanUpdateDTO planUpdateDto)
        {
            return ExecuteMethod<PlanController, PlanApiResponse, PlanReadDTO>(() => _planRepository.UpdatePlan(id, planUpdateDto));
        }

        [Authorize]
        [HttpPatch("{planId}")]
        public ActionResult PartialPlanUpdate(int planId, JsonPatchDocument<PlanUpdateDTO> patchDoc)
        {
            var planModelFromRepo = _planRepository.GetPlanById(planId);
            if (planModelFromRepo == null)
            {
                return ExecuteMethod<PlanController>(() => new NotPlanFoundException());
            }

            var planToPatch = _mapper.Map<PlanUpdateDTO>(planModelFromRepo);
            patchDoc.ApplyTo(planToPatch);

            if (!TryValidateModel(planToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(planToPatch, planModelFromRepo);

            return ExecuteMethod<PlanController>(() => _planRepository.PartiallyUpdate(planId, planToPatch));
        }

        [Authorize]
        [HttpDelete("{planId}")]
        public ActionResult DeletePlan(int planId)
        {
            return ExecuteMethod<PlanController>(() => _planRepository.DeletePlan(planId));
        }
    }
}
