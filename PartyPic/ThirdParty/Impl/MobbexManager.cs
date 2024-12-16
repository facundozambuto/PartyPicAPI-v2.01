using MercadoPago.Resource.PreApproval;
using PartyPic.Models.Subscriptions;
using System.Threading.Tasks;

namespace PartyPic.ThirdParty.Impl
{
    public class MobbexManager : IPaymentGatewayStrategy
    {
        public string CreateSubscription(MPSNewSubscriptionRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<MPSubscriptionResponse> GetSubscriptionAsync(string subscriptionId)
        {
            throw new System.NotImplementedException();
        }

        public Preapproval GetSubscriptionByExternalReference(string externalReference)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ToggleAutoRenewAsync(string subscriptionId, bool isAutoRenew)
        {
            throw new System.NotImplementedException();
        }
    }
}
