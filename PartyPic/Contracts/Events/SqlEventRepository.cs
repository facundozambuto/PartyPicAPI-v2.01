using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Categories;
using PartyPic.Contracts.Images;
using PartyPic.Contracts.Venues;
using PartyPic.DTOs.Events;
using PartyPic.Helpers;
using PartyPic.Models.Common;
using PartyPic.Models.Events;
using PartyPic.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using PartyPic.Contracts.Users;
using Microsoft.AspNetCore.Http;
using PartyPic.Models.Users;

namespace PartyPic.Contracts.Events
{
    public class SqlEventRepository : IEventRepository
    {
        private readonly EventContext _eventContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly VenueContext _venueContext;
        private readonly CategoryContext _categoryContext;
        private readonly ImagesContext _imagesContext;
        private readonly UserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlEventRepository(
            EventContext eventContext, 
            IMapper mapper, 
            IConfiguration config, 
            VenueContext venueContext, 
            CategoryContext categoryContext, 
            ImagesContext imagesContext,
            UserContext userContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventContext = eventContext;
            _mapper = mapper;
            _config = config;
            _venueContext = venueContext;
            _categoryContext = categoryContext;
            _imagesContext = imagesContext;
            _userContext = userContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public AllEventsResponse GetAllEvents()
        {
            var events = _mapper.Map<List<EventReadDTO>>(_eventContext.Events.ToList());

            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            var venues = _venueContext.Venues.Where(v => v.UserId == currentUser.UserId);

            if (currentUser.UserId != 1 && venues != null)
            {
                events = events.Where(ev => venues.Any(v => v.VenueId == ev.VenueId)).ToList();
            }

            foreach (EventReadDTO ev in events)
            {
                if (_venueContext.Venues.FirstOrDefault(v => v.VenueId == ev.VenueId) != null)
                {
                    ev.VenueName = _venueContext.Venues.FirstOrDefault(v => v.VenueId == ev.VenueId).Name;
                }
            }

            return new AllEventsResponse
            {
                Events = events
            };
        }

        public EventReadDTO GetEventById(int id)
        {
            var retrievedEvent = _eventContext.Events.AsNoTracking().FirstOrDefault(ev => ev.EventId.Equals(id));

            if (retrievedEvent == null)
            {
                throw new NotEventFoundException();
            }

            var evnt = _mapper.Map<EventReadDTO>(retrievedEvent);

            if (_venueContext.Venues.FirstOrDefault(v => v.VenueId == evnt.VenueId) != null)
            {
                evnt.VenueName = _venueContext.Venues.FirstOrDefault(v => v.VenueId == evnt.VenueId).Name;
            }

            if (_categoryContext.Categories.FirstOrDefault(c => c.CategoryId == evnt.CategoryId) != null)
            {
                evnt.CategoryDescription = _categoryContext.Categories.FirstOrDefault(c => c.CategoryId == evnt.CategoryId).Description;
            }

            return evnt;
        }

        public Event GetRawEventById(int id)
        {
            var retrievedEvent = _eventContext.Events.FirstOrDefault(ev => ev.EventId.Equals(id));

            if (retrievedEvent == null)
            {
                throw new NotEventFoundException();
            }

            return retrievedEvent;
        }

        public Event CreateEvent(Event ev)
        {
            this.ThrowExceptionIfArgumentIsNull(ev);
            this.ThrowExceptionIfPropertyIsIncorrect(ev, true, 0);

            ev.CreatedDatetime = DateTime.Now;

            ev.Code = this.GenerateRandomCode(ev);

            ev.QRCode = _config.GetValue<string>("QRCodeURL").Replace("{CODE}", ev.Code);

            _eventContext.Events.Add(ev);

            this.SaveChanges();

            var addedEvent = _eventContext.Events.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            this.CreateImagesEventFolder(addedEvent);

            return addedEvent;
        }

        public bool SaveChanges()
        {
            return (_eventContext.SaveChanges() >= 0);
        }

        public void DeleteEvent(int id)
        {
            var evt = _mapper.Map<Event>(this.GetRawEventById(id));

            if (evt == null)
            {
                throw new NotEventFoundException();
            }

            if (_imagesContext.Images.Any(i => i.EventId == evt.EventId))
            {
                throw new ExistingAsociatedImagesToEventException();
            }

            _eventContext.Events.Remove(evt);

            this.SaveChanges();
        }

        public EventReadDTO UpdateEvent(int id, EventUpdateDTO eventUpdateDto)
        {
            var retrievedEvent = this.GetRawEventById(id);

            retrievedEvent = _mapper.Map(eventUpdateDto, retrievedEvent);

            this.ThrowExceptionIfArgumentIsNull(retrievedEvent);
            this.ThrowExceptionIfPropertyIsIncorrect(retrievedEvent, false, id);

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

            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            var venues = _venueContext.Venues.Where(v => v.UserId == currentUser.UserId);

            if (currentUser.UserId != 1 && venues != null)
            {
                eventRows = eventRows.Where(ev => venues.Any(v => v.VenueId == ev.VenueId)).ToList();
            }

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                eventRows = _eventContext.Events.Where(u => u.Code.Contains(gridRequest.SearchPhrase)
                                                 || u.Description.Contains(gridRequest.SearchPhrase)
                                                 || u.Name.Contains(gridRequest.SearchPhrase)).ToList();
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

            var events = _mapper.Map<List<EventReadDTO>>(eventRows);

            foreach (EventReadDTO ev in events)
            {
                if (_venueContext.Venues.FirstOrDefault(v => v.VenueId == ev.VenueId) != null)
                {
                    ev.VenueName = _venueContext.Venues.FirstOrDefault(v => v.VenueId == ev.VenueId).Name;
                }

                if (_categoryContext.Categories.FirstOrDefault(c => c.CategoryId == ev.CategoryId) != null)
                {
                    ev.CategoryDescription = _categoryContext.Categories.FirstOrDefault(c => c.CategoryId == ev.CategoryId).Description;
                }
            }

            if (!string.IsNullOrEmpty(gridRequest.VenueId))
            {
                events = events.Where(ev => ev.VenueId == Convert.ToInt32(gridRequest.VenueId)).ToList();
            }

            if (!string.IsNullOrEmpty(gridRequest.EventId))
            {
                events = events.FindAll(ev => ev.EventId == Convert.ToInt32(gridRequest.EventId));
            }

            var eventGrid = new EventGrid
            {
                Rows = events,
                Total = _eventContext.Events.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return eventGrid;
        }

        public void SendInstructionsByEmail(int id)
        {
            var evt = this.GetEventById(id);

            var venue = _venueContext.Venues.FirstOrDefault(v => v.VenueId == evt.VenueId);

            var user = _userContext.Users.FirstOrDefault(u => u.UserId == venue.UserId);

            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress("PartyPic Admin", _config.GetValue<string>("EmailFromAdmin"));
            
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress(user.Name, user.Email);

            message.To.Add(to);

            message.Subject = _config.GetValue<string>("EmailSubject");

            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = _config.GetValue<string>("EmailTemplate").Replace("***eventName***", evt.Name).Replace("***qrCode***", evt.QRCode).Replace("***eventCode***", evt.Code);

            message.Body = bodyBuilder.ToMessageBody();

            foreach (var body in message.BodyParts.OfType<TextPart>())
                body.ContentTransferEncoding = ContentEncoding.Base64;

            SmtpClient client = new SmtpClient();
            client.CheckCertificateRevocation = false;
            client.Connect(_config.GetValue<string>("SMTPServer"), 2525, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(_config.GetValue<string>("SMTPUser"), _config.GetValue<string>("SMTPPassword"));

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();
        }

        public EventReadDTO GetEventByEventCode(string eventCode)
        {
            var retrievedEvent = _eventContext.Events.AsNoTracking().FirstOrDefault(ev => ev.Code.ToLower() == eventCode.ToLower());

            if (retrievedEvent == null)
            {
                throw new NotEventFoundException();
            }

            var evnt = _mapper.Map<EventReadDTO>(retrievedEvent);

            if (_venueContext.Venues.FirstOrDefault(v => v.VenueId == evnt.VenueId) != null)
            {
                evnt.VenueName = _venueContext.Venues.FirstOrDefault(v => v.VenueId == evnt.VenueId).Name;
            }

            if (_categoryContext.Categories.FirstOrDefault(c => c.CategoryId == evnt.CategoryId) != null)
            {
                evnt.CategoryDescription = _categoryContext.Categories.FirstOrDefault(c => c.CategoryId == evnt.CategoryId).Description;
            }

            return evnt;
        }

        private string GenerateRandomCode(Event ev)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789" + ev.Description + ev.VenueId;
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars).Replace(" ","").Trim();

            return finalString;
        }

        private void CreateImagesEventFolder(Event addedEvent)
        {
            string path = _config.GetValue<string>("DirectoryEventImagesPath") + addedEvent.EventId;

            try
            {
                if (Directory.Exists(path))
                {
                    throw new AlreadyExistingElementException();
                }

                DirectoryInfo dir = Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ThrowExceptionIfPropertyIsIncorrect(Event ev, bool isNew, int id)
        {
            if (!isNew)
            {
                if (ev.CreatedDatetime.Value != _eventContext.Events.FirstOrDefault(e => e.EventId == id).CreatedDatetime.Value)
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

            if (string.IsNullOrEmpty(ev.Enabled.ToString()))
            {
                throw new ArgumentNullException(nameof(ev.Enabled));
            }

            if (!_venueContext.Venues.Any(v => v.VenueId == ev.VenueId))
            {
                throw new ArgumentNullException(nameof(ev.VenueId));
            }
        }
    }
}
