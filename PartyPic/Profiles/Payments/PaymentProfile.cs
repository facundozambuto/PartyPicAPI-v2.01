namespace PartyPic.Profiles.Payments
{
    using AutoMapper;
    using PartyPic.DTOs.Payments;
    using PartyPic.Models.Payments;

    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentReadDTO>();
            CreateMap<PaymentCreateDTO, Payment>();
            CreateMap<PaymentGrid, PaymentReadDTOGrid>();
            CreateMap<PaymentReadDTOGrid, PaymentGrid>();
            CreateMap<PaymentUpdateDTO, Payment>();
            CreateMap<Payment, PaymentUpdateDTO>();
            CreateMap<AllPaymentsResponse, GetAllPaymentsApiResponse>();
            CreateMap<GetAllPaymentsApiResponse, AllPaymentsResponse>();
            CreateMap<AllPaymentsResponse, GetAllPaymentsApiResponse>();
            CreateMap<GetAllPaymentsApiResponse, AllPaymentsResponse>();
            CreateMap<PaymentGrid, GridPaymentApiResponse>();
            CreateMap<GridPaymentApiResponse, PaymentGrid>();
            CreateMap<PaymentApiResponse, Payment>();
            CreateMap<Payment, PaymentApiResponse>();
        }
    }
}
