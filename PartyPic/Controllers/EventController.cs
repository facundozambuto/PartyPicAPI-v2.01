using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Logger;
using PartyPic.DTOs.Events;
using PartyPic.Models.Common;
using PartyPic.Models.Events;
using PartyPic.Models.Exceptions;

namespace PartyPic.Controllers
{
    [EnableCors("CorsApi")]
    [Route("api/events")]
    [ApiController]
    public class EventController : PartyPicControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly Contracts.Logger.ILoggerManager _logger;

        public EventController(IEventRepository eventRepository, IMapper mapper, IConfiguration config, ILoggerManager logger) : base(mapper, config, logger)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetAllEvents()
        {
            return ExecuteMethod<EventController, GetAllEventsApiResponse, AllEventsResponse>(() => _eventRepository.GetAllEvents());
        }

        [HttpGet]
        [Route("~/api/events/grid")]
        public ActionResult<EventGrid> GetAllEventsForGrid([FromQuery] int current,
                                                            [FromQuery] int rowCount,
                                                            [FromQuery] string searchPhrase,
                                                            [FromQuery] string sortBy,
                                                            [FromQuery] string orderBy,
                                                            [FromQuery] string eventId,
                                                            [FromQuery] string venueId)
        {
            GridRequest gridRequest = new GridRequest
            {
                Current = current,
                RowCount = rowCount,
                SearchPhrase = searchPhrase,
                SortBy = sortBy,
                OrderBy = orderBy,
                EventId = eventId,
                VenueId = venueId
            };

            return ExecuteMethod<EventController, GridEventApiResponse, EventGrid>(() => _eventRepository.GetAllEventsForGrid(gridRequest));
        }

        [HttpGet("{id}", Name = "GetEventById")]
        public ActionResult<EventReadDTO> GetEventById(int id)
        {
            return ExecuteMethod<EventController, EventApiResponse, EventReadDTO>(() => _eventRepository.GetEventById(id));
        }

        [HttpPost]
        public ActionResult<Event> CreateEvent(EventCreateDTO eventCreateDTO)
        {
            var eventModel = _mapper.Map<Event>(eventCreateDTO);

            return ExecuteMethod<EventController, EventApiResponse, Event>(() => _eventRepository.CreateEvent(eventModel));
        }

        [HttpPut("{id}")]
        public ActionResult UpdateEvent(int id, EventUpdateDTO eventUpdateDto)
        {
            return ExecuteMethod<EventController, EventApiResponse, EventReadDTO>(() => _eventRepository.UpdateEvent(id, eventUpdateDto));
        }

        [HttpPatch("{id}")]
        public ActionResult PartialEventUpdate(int id, JsonPatchDocument<EventUpdateDTO> patchDoc)
        {
            var eventModelFromRepo = _eventRepository.GetEventById(id);
            if (eventModelFromRepo == null)
            {
                return ExecuteMethod<EventController>(() => new NotEventFoundException());
            }

            var eventToPatch = _mapper.Map<EventUpdateDTO>(eventModelFromRepo);
            patchDoc.ApplyTo(eventToPatch);

            if (!TryValidateModel(eventToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(eventToPatch, eventModelFromRepo);

            return ExecuteMethod<EventController>(() => _eventRepository.PartiallyUpdate(id, eventToPatch));
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteEvent(int id)
        {
            return ExecuteMethod<EventController>(() => _eventRepository.DeleteEvent(id));
        }

        [HttpPost("{eventId}")]
        [Route("~/api/events/sendInstructions")]
        public ActionResult SendInstructionsByEmail(EventReadDTO evt)
        {
            return ExecuteMethod<EventController>(() => _eventRepository.SendInstructionsByEmail(evt.EventId));
        }

        [HttpGet("GetByEventCode/{eventCode}", Name = "GetEventByEventCode")]
        public ActionResult<EventReadDTO> GetEventByEventCode(string eventCode)
        {
            return ExecuteMethod<EventController, EventApiResponse, EventReadDTO>(() => _eventRepository.GetEventByEventCode(eventCode));
        }
    }
}
