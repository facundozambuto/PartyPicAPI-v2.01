using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Venues;
using PartyPic.DTOs.Venues;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Venues;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/venues")]
    [ApiController]
    public class VenueController : PartyPicControllerBase
    {
        private readonly IVenueRepository _venueRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public VenueController(IVenueRepository venueRepository, IMapper mapper, IConfiguration config, Contracts.Logger.ILoggerManager logger) : base(mapper, config, logger)
        {
            _venueRepository = venueRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllVenues()
        {
            return ExecuteMethod<VenueController, GetAllVenuesApiResponse, AllVenuesResponse>(() => _venueRepository.GetAllVenues());
        }

        [HttpGet]
        [Route("~/api/venues/grid")]
        [Authorize]
        public ActionResult<VenueReadDTOGrid> GetAllVenuesForGrid([FromQuery] int current, [FromQuery] int rowCount, [FromQuery] string searchPhrase, [FromQuery] string sortBy, string orderBy)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy
            };

            return ExecuteMethod<VenueController, GridVenueApiResponse, VenueReadDTOGrid>(() => _venueRepository.GetAllVenuesForGrid(gridRequest));
        }

        [HttpGet("{id}", Name = "GetVenueById")]
        [Authorize]
        public ActionResult<Venue> GetVenueById(int venueId)
        {
            return ExecuteMethod<VenueController, VenueApiResponse, Venue>(() => _venueRepository.GetVenueById(venueId));
        }

        [HttpGet("{id}", Name = "GetVenueManager")]
        [Route("~/api/venues/venueManager")]
        [Authorize]
        public ActionResult<VenueApiResponse> GetVenueManager([FromQuery] int venueId)
        {
            return ExecuteMethod<VenueController, VenueApiResponse, VenueReadDTO>(() => _venueRepository.GetVenueFullData(venueId));
        }

        [HttpPost]
        [Authorize]
        public ActionResult<Venue> CreateVenue(VenueCreateDTO venueCreateDTO)
        {
            var venueModel = _mapper.Map<Venue>(venueCreateDTO);

            return ExecuteMethod<VenueController, VenueApiResponse, Venue>(() => _venueRepository.CreateVenue(venueModel));
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdateVenue(int id, VenueUpdateDTO venueUpdateDto)
        {
            return ExecuteMethod<VenueController, VenueApiResponse, Venue>(() => _venueRepository.UpdateVenue(id, venueUpdateDto));
        }

        [HttpPatch("{id}")]
        [Authorize]
        public ActionResult PartialVenueUpdate(int venueId, JsonPatchDocument<VenueUpdateDTO> patchDoc)
        {
            var venueModelFromRepo = _venueRepository.GetVenueById(venueId);
            if (venueModelFromRepo == null)
            {
                return ExecuteMethod<VenueController>(() => new NotVenueFoundException());
            }

            var venueToPatch = _mapper.Map<VenueUpdateDTO>(venueModelFromRepo);
            patchDoc.ApplyTo(venueToPatch);

            if (!TryValidateModel(venueToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(venueToPatch, venueModelFromRepo);

            return ExecuteMethod<VenueController>(() => _venueRepository.PartiallyUpdate(venueId, venueToPatch));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteVenue(int venueId)
        {
            return ExecuteMethod<VenueController>(() => _venueRepository.DeleteVenue(venueId));
        }
    }
}
