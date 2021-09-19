namespace PartyPic.Models.Common
{
    public class GridRequest : ApiRequest
    {
        public int Current { get; set; }
        public int RowCount { get; set; }
        public string OrderBy { get; set; }
        public string SortBy { get; set; }
        public string SearchPhrase { get; set; }
        public string EventId { get; set; }
        public string VenueId { get; set; }
    }
}
