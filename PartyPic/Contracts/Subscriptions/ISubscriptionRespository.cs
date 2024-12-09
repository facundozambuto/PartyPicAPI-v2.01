using PartyPic.DTOs.Subscriptions;
using PartyPic.Models.Subscriptions;
using PartyPic.Models.Common;
using System.Threading.Tasks;

namespace PartyPic.Contracts.Subscriptions
{
    public interface ISubscriptionRepository
    {
        AllSubscriptionsResponse GetAllSubscriptions();
        Task<SubscriptionReadDTO> GetSubscriptionByIdAsync(int id);
        SubscriptionReadDTO CreateSubscription(SubscriptionCreateDTO subscription);
        bool SaveChanges();
        void DeleteSubscription(int id);
        Task<SubscriptionReadDTO> UpdateSubscriptionAsync(int id, SubscriptionUpdateDTO subscription);
        void PartiallyUpdate(int id, SubscriptionUpdateDTO subscription);
        SubscriptionGrid GetAllSubscriptionsForGrid(GridRequest gridRequest);
        Task<AllSubscriptionsResponse> GetAllMySubscriptionsAsync();
        SubscriptionReadDTO ConfirmSubscription(string externalReference);
    }
}
