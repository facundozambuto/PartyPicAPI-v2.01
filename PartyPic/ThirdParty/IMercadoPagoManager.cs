using MercadoPago.Resource.PreApproval;
using PartyPic.Models.Subscriptions;
using System.Threading.Tasks;

namespace PartyPic.ThirdParty
{
    public interface IMercadoPagoManager
    {
        string CreateSubscription(MPSNewSubscriptionRequest request);
        Preapproval GetSubscriptionByExternalReference(string externalReference);
        Task<MPSubscriptionResponse> GetSubscriptionAsync(string subscriptionId);
    }
}
