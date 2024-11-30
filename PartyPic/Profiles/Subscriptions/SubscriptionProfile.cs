namespace PartyPic.Profiles.Subscriptions
{
    using AutoMapper;
    using PartyPic.DTOs.Subscriptions;
    using PartyPic.Models.Subscriptions;

    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionReadDTO>();
            CreateMap<SubscriptionCreateDTO, Subscription>();
            CreateMap<SubscriptionGrid, SubscriptionReadDTOGrid>();
            CreateMap<SubscriptionReadDTOGrid, SubscriptionGrid>();
            CreateMap<SubscriptionUpdateDTO, Subscription>();
            CreateMap<Subscription, SubscriptionUpdateDTO>();
            CreateMap<AllSubscriptionsResponse, GetAllSubscriptionsApiResponse>();
            CreateMap<GetAllSubscriptionsApiResponse, AllSubscriptionsResponse>();
            CreateMap<AllSubscriptionsResponse, GetAllSubscriptionsApiResponse>();
            CreateMap<GetAllSubscriptionsApiResponse, AllSubscriptionsResponse>();
            CreateMap<SubscriptionGrid, GridSubscriptionApiResponse>();
            CreateMap<GridSubscriptionApiResponse, SubscriptionGrid>();
            CreateMap<SubscriptionApiResponse, Subscription>();
            CreateMap<Subscription, SubscriptionApiResponse>();
        }
    }
}
