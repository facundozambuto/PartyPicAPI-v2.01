using PartyPic.DTOs.Subscriptions;
using PartyPic.Models.Subscriptions;
using PartyPic.Models.Common;

namespace PartyPic.Contracts.Subscriptions
{
    public interface ISubscriptionRespository
    {
        AllSubscriptionsResponse GetAllSubscriptions();
        Subscription GetSubscriptionById(int id);
        Subscription CreateSubscription(Subscription category);
        bool SaveChanges();
        void DeleteSubscription(int id);
        Subscription UpdateSubscription(int id, SubscriptionUpdateDTO category);
        void PartiallyUpdate(int id, SubscriptionUpdateDTO category);
        SubscriptionGrid GetAllSubscriptionsForGrid(GridRequest gridRequest);
    }
}
