using PartyPic.Models.Common;

namespace PartyPic.DTOs
{
    public class GetReportsApiResponse : ApiResponse
    {
        public int AmountOfNewEvents { get; set; }
        public int AmountOfNewVenues { get; set; }
        public int AmountOfNewVenuesManagers { get; set; }
        public int AmountOfUploadedImages { get; set; }
    }
}
