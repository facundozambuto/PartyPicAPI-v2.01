using AutoMapper;
using Microsoft.AspNetCore.Http;
using PartyPic.Contracts.Users;
using PartyPic.DTOs.Venues;
using PartyPic.Helpers;
using PartyPic.Models.Common;
using PartyPic.Models.Exceptions;
using PartyPic.Models.Users;
using PartyPic.Models.Venues;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.Contracts.Venues
{
    public class SqlVenueRepository : IVenueRepository
    {
        private readonly VenueContext _venueContext;
        private readonly IMapper _mapper;
        private readonly UserContext _userContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SqlVenueRepository(VenueContext venueContext, IMapper mapper, UserContext userContext, IHttpContextAccessor httpContextAccessor)
        {
            _venueContext = venueContext;
            _mapper = mapper;
            _userContext = userContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Venue CreateVenue(Venue venue)
        {
            this.ThrowExceptionIfArgumentIsNull(venue);
            this.ThrowExceptionIfPropertyAlreadyExists(venue, true, 0);
            this.ThrowExceptionIfUserDoesNotExist(venue.UserId);

            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            if (currentUser.RoleId == 2 && currentUser.UserId != venue.UserId)
            {
                throw new InvalidOperationException();
            }

            venue.CreatedDatetime = DateTime.Now;

            _venueContext.Venues.Add(venue);

            this.SaveChanges();

            var addedVenue = _venueContext.Venues.OrderByDescending(u => u.CreatedDatetime).FirstOrDefault();

            return addedVenue;
        }

        public AllVenuesResponse GetAllVenues()
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];
            var listOfVenues = new List<Venue>();

            listOfVenues = _venueContext.Venues.ToList();

            if (currentUser.RoleId == 2)
            {
                listOfVenues = listOfVenues.Where(v => v.UserId == currentUser.UserId).ToList();
            }

            return new AllVenuesResponse
            {
                Venues = listOfVenues
            };
        }

        public Venue GetVenueById(int id)
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            var venue = _venueContext.Venues.FirstOrDefault(venue => venue.VenueId == id);

            if (venue == null)
            {
                throw new NotVenueFoundException();
            }

            if (currentUser.RoleId == 2 && venue.UserId != currentUser.UserId)
            {
                throw new InvalidOperationException();
            }

            return venue;
        }

        public bool SaveChanges()
        {
            return (_venueContext.SaveChanges() >= 0);
        }

        public void DeleteVenue(int id)
        {
            var venue = this.GetVenueById(id);

            if (venue == null)
            {
                throw new NotVenueFoundException();
            }

            _venueContext.Venues.Remove(venue);

            this.SaveChanges();
        }

        public void PartiallyUpdate(int id, VenueUpdateDTO venue)
        {
            this.UpdateVenue(id, venue);

            this.SaveChanges();
        }

        public Venue UpdateVenue(int id, VenueUpdateDTO venueUpdateDto)
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];

            var venue = _mapper.Map<Venue>(venueUpdateDto);

            var retrievedVenue = this.GetVenueById(id);

            if (retrievedVenue == null)
            {
                throw new NotVenueFoundException();
            }

            if (currentUser.RoleId == 2 && retrievedVenue.UserId != currentUser.UserId)
            {
                throw new InvalidOperationException();
            }

            this.ThrowExceptionIfArgumentIsNull(venue);
            this.ThrowExceptionIfPropertyAlreadyExists(venue, false, id);
            this.ThrowExceptionIfUserDoesNotExist(venue.UserId);

            _mapper.Map(venueUpdateDto, retrievedVenue);

            _venueContext.Venues.Update(retrievedVenue);

            this.SaveChanges();

            return this.GetVenueById(id);
        }

        public VenueReadDTOGrid GetAllVenuesForGrid(GridRequest gridRequest)
        {
            var currentUser = (User)_httpContextAccessor.HttpContext.Items["User"];
            var listOfVenues = new List<Venue>();

            listOfVenues = _venueContext.Venues.ToList();

            if (currentUser.RoleId == 2)
            {
                listOfVenues = listOfVenues.Where(v => v.UserId == currentUser.UserId).ToList();
            }

            var venueRows = _mapper.Map<List<VenueReadDTO>>(listOfVenues);

            foreach (VenueReadDTO ve in venueRows)
            {
                if (_userContext.Users.FirstOrDefault(u => u.UserId == ve.UserId) != null)
                {
                    ve.ManagerName = _userContext.Users.FirstOrDefault(u => u.UserId == ve.UserId).Name;
                }
            }

            if (!string.IsNullOrEmpty(gridRequest.SearchPhrase))
            {
                venueRows = _mapper.Map<List<VenueReadDTO>>(_venueContext.Venues.Where(v => v.Address.Contains(gridRequest.SearchPhrase)
                                                 || v.Name.Contains(gridRequest.SearchPhrase)).ToList());
            }

            if (gridRequest.RowCount != -1 && _venueContext.Venues.Count() > gridRequest.RowCount && gridRequest.Current > 0 && venueRows.Count > 0)
            {
                var offset = gridRequest.RowCount;
                var index = (gridRequest.Current - 1) * gridRequest.RowCount;

                if ((venueRows.Count % gridRequest.RowCount) != 0 && (venueRows.Count / gridRequest.RowCount) < gridRequest.Current)
                {
                    offset = venueRows.Count % gridRequest.RowCount;
                }

                venueRows = venueRows.GetRange(index, offset);
            }

            if (!string.IsNullOrEmpty(gridRequest.SortBy) && !string.IsNullOrEmpty(gridRequest.OrderBy))
            {
                gridRequest.SortBy = WordingHelper.FirstCharToUpper(gridRequest.SortBy);

                venueRows = venueRows
                                .OrderBy(m => m.GetType()
                                                .GetProperties()
                                                .First(n => n.Name == gridRequest.SortBy)
                                .GetValue(m, null))
                                .ToList();

                if (gridRequest.OrderBy.ToLowerInvariant() == "desc")
                {
                    venueRows.Reverse();
                }
            }

            var venueGrid = new VenueReadDTOGrid
            {
                Rows = venueRows,
                Total = _venueContext.Venues.Count(),
                Current = gridRequest.Current,
                RowCount = gridRequest.RowCount
            };

            return venueGrid;
        }

        private void ThrowExceptionIfArgumentIsNull(Venue venue)
        {
            if (venue == null)
            {
                throw new ArgumentNullException(nameof(venue));
            }

            if (string.IsNullOrEmpty(venue.Address))
            {
                throw new ArgumentNullException(nameof(venue.Address));
            }

            if (string.IsNullOrEmpty(venue.Name))
            {
                throw new ArgumentNullException(nameof(venue.Name));
            }

            if (string.IsNullOrEmpty(venue.Name))
            {
                throw new ArgumentNullException(nameof(venue.Name));
            }

            if (string.IsNullOrEmpty(venue.Phone))
            {
                throw new ArgumentNullException(nameof(venue.Phone));
            }

            if (venue.CreatedDatetime == null)
            {
                throw new ArgumentNullException(nameof(venue.CreatedDatetime));
            }
        }

        private void ThrowExceptionIfPropertyAlreadyExists(Venue venue, bool isNew, int venueId)
        {
            var allVenues = _venueContext.Venues.ToList();

            if (!isNew)
            {
                allVenues = allVenues.Where(v => v.VenueId != venueId).ToList();
            }

            if (allVenues.Any(v => v.Name == venue.Name))
            {
                throw new AlreadyExistingElementException();
            }
        }

        private void ThrowExceptionIfUserDoesNotExist(int userId)
        {
            if (!_userContext.Users.Any(u => u.UserId == userId))
            {
                throw new NotUserFoundException();
            }
        }

        public VenueReadDTO GetVenueFullData(int venueId)
        {
            var venue = _venueContext.Venues.FirstOrDefault(v => v.VenueId == venueId);

            if (venue == null)
            {
                throw new NotVenueFoundException();
            }

            var venueManager = _userContext.Users.FirstOrDefault(u => u.UserId == venue.UserId);

            if (venueManager == null)
            {
                throw new NotUserFoundException();
            }

            return new VenueReadDTO
            {
                Name = venue.Name,
                Address = venue.Address,
                Phone = venue.Phone,
                ManagerName = venueManager.Name,
                ManagerEmail = venueManager.Email,
                ManagerMobilePhone = venueManager.MobilePhone,
                ManagerAddress = venueManager.Address,
                ManagerCuil = venueManager.Cuil
            };
        }
    }
}
