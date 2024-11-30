namespace PartyPic.DTOs.Payments
{
    using PartyPic.Models.Common;
    using System.Collections.Generic;

    public class GridPaymentApiResponse : ApiResponse
    {
        public List<PaymentReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
