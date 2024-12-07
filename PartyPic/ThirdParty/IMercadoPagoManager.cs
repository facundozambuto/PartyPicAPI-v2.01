using MercadoPago.Resource.PreApproval;
using PartyPic.Models.Subscriptions;

namespace PartyPic.ThirdParty
{
    public interface IMercadoPagoManager
    {
        string CreateSubscription(MPSNewSubscriptionRequest request);
        Preapproval GetSubscriptionByExternalReference(string externalReference);
    }
}
