using AutoMapper;
using Microsoft.AspNetCore.Cors;
using PartyPic.Contracts.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using PartyPic.Models.Common;
using Microsoft.Extensions.Configuration;
using PartyPic.DTOs.Subscriptions;
using PartyPic.Models.Subscriptions;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/Subscriptions")]
    [ApiController]
    public class SubscriptionController : PartyPicControllerBase
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public SubscriptionController(ISubscriptionRepository subscriptionRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllSubscriptions()
        {
            return ExecuteMethod<SubscriptionController, GetAllSubscriptionsApiResponse, AllSubscriptionsResponse>(() => _subscriptionRepository.GetAllSubscriptions());
        }

        [HttpGet]
        [Authorize]
        [Route("~/api/Subscriptions/mysubs")]
        public ActionResult GetAllMySubscriptions()
        {
            return ExecuteMethod<SubscriptionController, GetAllSubscriptionsApiResponse, AllSubscriptionsResponse>(
                () => _subscriptionRepository.GetAllMySubscriptionsAsync().GetAwaiter().GetResult()
            );
        }

        [Authorize]
        [HttpGet]
        [Route("~/api/Subscriptions/grid")]
        public ActionResult<SubscriptionGrid> GetAllSubscriptionsForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<SubscriptionController, GridSubscriptionApiResponse, SubscriptionGrid>(() => _subscriptionRepository.GetAllSubscriptionsForGrid(gridRequest));
        }

        [Authorize]
        [HttpPost]
        public ActionResult<SubscriptionReadDTO> CreatePlan(SubscriptionCreateDTO subscriptionCreateDTO)
        {
            return ExecuteMethod<PlanController, SubscriptionReadDTO, SubscriptionReadDTO>(() => _subscriptionRepository.CreateSubscription(subscriptionCreateDTO));
        }

        [Authorize]
        [HttpGet("{SubscriptionId}", Name = "GetSubscriptionById")]
        public ActionResult<SubscriptionReadDTO> GetSubscriptionById(int subscriptionId)
        {
            return ExecuteMethod<SubscriptionController, SubscriptionReadDTO, SubscriptionReadDTO>(
                () => _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId).GetAwaiter().GetResult()
            );
        }

        [Authorize]
        [Route("~/api/Subscriptions/Confirm")]
        public ActionResult<SubscriptionReadDTO> ConfirmSubscription([FromQuery] string externalReference)
        {
            return ExecuteMethod<SubscriptionController, SubscriptionReadDTO, SubscriptionReadDTO>(() => _subscriptionRepository.ConfirmSubscription(externalReference));
        }
    }
}
