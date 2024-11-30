namespace PartyPic.DTOs.Payments
{
    using PartyPic.Models.Common;
    using System.Collections.Generic;

    public class GetAllPaymentsApiResponse : ApiResponse
    {
        public List<PaymentReadDTO> Payments { get; set; }
    }
}
