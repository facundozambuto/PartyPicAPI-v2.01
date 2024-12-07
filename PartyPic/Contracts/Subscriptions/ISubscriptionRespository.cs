using PartyPic.DTOs.Subscriptions;
using PartyPic.Models.Subscriptions;
using PartyPic.Models.Common;

namespace PartyPic.Contracts.Subscriptions
{
    public interface ISubscriptionRepository
    {
        AllSubscriptionsResponse GetAllSubscriptions();
        Subscription GetSubscriptionById(int id);
        SubscriptionReadDTO CreateSubscription(SubscriptionCreateDTO subscription);
        bool SaveChanges();
        void DeleteSubscription(int id);
        Subscription UpdateSubscription(int id, SubscriptionUpdateDTO subscription);
        void PartiallyUpdate(int id, SubscriptionUpdateDTO subscription);
        SubscriptionGrid GetAllSubscriptionsForGrid(GridRequest gridRequest);
        AllSubscriptionsResponse GetAllMySubscriptions();
        SubscriptionReadDTO ConfirmSubscription(string externalReference);
    }
}
