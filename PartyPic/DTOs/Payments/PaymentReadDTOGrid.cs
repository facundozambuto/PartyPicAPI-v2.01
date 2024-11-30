namespace PartyPic.DTOs.Payments
{
    using System.Collections.Generic;

    public class PaymentReadDTOGrid
    {
        public List<PaymentReadDTO> Rows { get; set; }
        public int Total { get; set; }
        public int RowCount { get; set; }
        public int Current { get; set; }
    }
}
