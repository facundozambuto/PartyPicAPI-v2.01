using PartyPic.DTOs.Venues;
using PartyPic.Models.Common;
using PartyPic.Models.Venues;

namespace PartyPic.Contracts.Venues
{
    public interface IVenueRepository
    {
        AllVenuesResponse GetAllVenues();
        Venue GetVenueById(int venueId);
        Venue CreateVenue(Venue venue);
        bool SaveChanges();
        void DeleteVenue(int venueId);
        Venue UpdateVenue(int venueId, VenueUpdateDTO venue);
        void PartiallyUpdate(int venueId, VenueUpdateDTO veue);
        VenueReadDTOGrid GetAllVenuesForGrid(GridRequest gridRequest);
        VenueReadDTO GetVenueFullData(int venueId);
    }
}
