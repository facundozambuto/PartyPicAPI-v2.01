using AutoMapper;
using Microsoft.Extensions.Configuration;
using PartyPic.Contracts.Events;
using PartyPic.Contracts.Images;
using PartyPic.Contracts.Users;
using PartyPic.Contracts.Venues;
using PartyPic.Models.Reports;
using System;
using System.Linq;

namespace PartyPic.Contracts.Reports
{
    public class SqlReportsRepository : IReportsRepository
    {
        private readonly EventContext _eventContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly VenueContext _venueContext;
        private readonly ImagesContext _imagesContext;
        private readonly UserContext _userContext;

        public SqlReportsRepository(
            EventContext eventContext,
            IMapper mapper,
            IConfiguration config,
            VenueContext venueContext,
            ImagesContext imagesContext,
            UserContext userContext)
        {
            _eventContext = eventContext;
            _mapper = mapper;
            _config = config;
            _venueContext = venueContext;
            _imagesContext = imagesContext;
            _userContext = userContext;
        }


        public ReportsResponse GetReports()
        {
            var newVenues = _venueContext.Venues.Count(v => v.CreatedDatetime > DateTime.Now.AddDays(-30));

            var newVenueManagers = _userContext.Users.Count(u => u.CreatedDatetime > DateTime.Now.AddDays(-30) && u.RoleId == 2);

            var newEvents = _eventContext.Events.Count(e => e.CreatedDatetime > DateTime.Now.AddDays(-30));

            var uploadedImages = _imagesContext.Images.Count(e => e.CreatedDatetime > DateTime.Now.AddDays(-30));

            return new ReportsResponse
            {
                AmountOfNewEvents = newEvents,
                AmountOfNewVenues = newVenues,
                AmountOfNewVenuesManagers = newVenueManagers,
                AmountOfUploadedImages = uploadedImages
            };
        }
    }
}
