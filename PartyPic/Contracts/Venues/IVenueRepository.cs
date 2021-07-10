using PartyPic.DTOs.Venues;
using PartyPic.Models.Common;
using PartyPic.Models.Venues;

namespace PartyPic.Contracts.Venues
{
    public interface IVenueRepository
    {
        AllVenuesResponse GetAllVenues();
        Venue GetVenueById(int id);
        Venue CreateVenue(Venue venue);
        bool SaveChanges();
        void DeleteVenue(int id);
        Venue UpdateVenue(int id, VenueUpdateDTO venue);
        void PartiallyUpdate(int id, VenueUpdateDTO veue);
        VenueGrid GetAllVenuesForGrid(GridRequest gridRequest);
    }
}
