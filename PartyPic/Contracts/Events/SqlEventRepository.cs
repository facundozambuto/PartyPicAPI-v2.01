using AutoMapper;
using PartyPic.DTOs.Events;
using PartyPic.Helpers;
using PartyPic.Models.Common;
using PartyPic.Models.Events;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.Events
{
    public class SqlEventRepository : IEventRepository
    {
        private EventContext _eventContext;
        private readonly IMapper _mapper;

        public SqlEventRepository(EventContext eventContext, IMapper mapper)
        {
            _eventContext = eventContext;
            _mapper = mapper;
        }

        public AllEventsResponse GetAllEvents()
        {
            return new AllEventsResponse
            {
                Events = _eventContext.Events.ToList()
            };
        }

        public Event GetEventById(int id)
        {
            var user = _eventContext.Events.FirstOrDefault(ev => ev.EventId == id);

            if (user == null)
            {
                throw new NotEventFoundException();
            }

            return user;
        }

        public Event CreateEvent(Event ev)
        {
            this.ThrowExceptionIfArgumentIsNull(ev);
            this.ThrowExceptionIfPropertyIsIncorrect(ev, true, 0);

            ev.CreatedDatetime = DateTime.Now;

            _eventContext.Events.Add(ev);
            this.SaveChanges();

            var addedEvent = _eventContext.Events.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedEvent;
        }

        public bool SaveChanges()
        {
            return (_eventContext.SaveChanges() >= 0);
        }

        public void DeleteEvent(int id)
        {
            var user = this.GetEventById(id);

            if (user == null)
            {
                throw new NotEventFoundException();
            }

            _eventContext.Events.Remove(user);

            this.SaveChanges();
        }

        public Event UpdateEvent(int id, EventUpdateDTO eventUpdateDto)
        {
            var ev = _mapper.Map<Event>(eventUpdateDto);

            var retrievedEvent = this.GetEventById(id);

            if (retrievedEvent == null)
            {
                throw new NotEventFoundException();
            }

            this.ThrowExceptionIfArgumentIsNull(ev);
            this.ThrowExceptionIfPropertyIsIncorrect(ev, false, id);

            _mapper.Map(eventUpdateDto, retrievedEvent);

            _eventContext.Events.Update(retrievedEvent);

            this.SaveChanges();

            return this.GetEventById(id);
        }

        public void PartiallyUpdate(int id, EventUpdateDTO ev)
        {
            this.UpdateEvent(id, ev);

            this.SaveChanges();
        }

        public EventGrid GetAllEventsForGrid(GridRequest gridRequest)
        {
            var eventRows = new List<Event>();

            eventRows = _eventContext.Events.ToList();

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                eventRows = _eventContext.Events.Where(u => u.Code.Contains(gridRequest.SearchPhrase)
                                                 || u.Description.Contains(gridRequest.SearchPhrase)).ToList();
            }


            if (gridRequest.RowCount != -1 && _eventContext.Events.Count() > gridRequest.RowCount && gridRequest.Current > 0 && eventRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((eventRows.Count % gridRequest.RowCount) != 0 && (eventRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = eventRows.Count % gridRequest.RowCount;
                }

                eventRows = eventRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                eventRows = eventRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    eventRows.Reverse();
                }
            }

            var eventGrid = new EventGrid
            {
                Rows = eventRows,
                Total = _eventContext.Events.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return eventGrid;
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Event ev, bool isNew, int id)
        {
            if (!isNew)
            {
                if (ev.CreatedDatetime.Date != _eventContext.Events.FirstOrDefault(e => e.EventId == id).CreatedDatetime.Date)
                {
                    throw new PropertyIncorrectException();
                }
            }
        }

        private void ThrowExceptionIfArgumentIsNull(Event ev)
        {
            if (ev == null)
            {
                throw new ArgumentNullException(nameof(ev));
            }

            if (string.IsNullOrEmpty(ev.Code))
            {
                throw new ArgumentNullException(nameof(ev.Code));
            }

            if (string.IsNullOrEmpty(ev.Description))
            {
                throw new ArgumentNullException(nameof(ev.Description));
            }

            if (ev.VenueId == 0)
            {
                throw new ArgumentNullException(nameof(ev.VenueId));
            }

            if (ev.StartDatetime == null)
            {
                throw new ArgumentNullException(nameof(ev.StartDatetime));
            }

            if (ev.LastRequest == null)
            {
                throw new ArgumentNullException(nameof(ev.LastRequest));
            }

            if (string.IsNullOrEmpty(ev.QRCode))
            {
                throw new ArgumentNullException(nameof(ev.QRCode));
            }

            if (string.IsNullOrEmpty(ev.VenueId.ToString()))
            {
                throw new ArgumentNullException(nameof(ev.VenueId));
            }

            if (string.IsNullOrEmpty(ev.Enabled.ToString()))
            {
                throw new ArgumentNullException(nameof(ev.Enabled));
            }

            if (ev.CreatedDatetime == null)
            {
                throw new ArgumentNullException(nameof(ev.CreatedDatetime));
            }
        }
    }
}
