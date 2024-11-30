namespace PartyPic.Models.Payments
{
    using PartyPic.Models.Common;
    using System.Collections.Generic;

    public class PaymentGrid : ApiResponse
    {
        public List<Payment> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
