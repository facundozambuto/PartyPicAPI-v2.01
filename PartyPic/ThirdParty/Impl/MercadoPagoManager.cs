using PartyPic.Models.Subscriptions;
using MercadoPago.Client.Preapproval;
using MercadoPago.Config;
using MercadoPago.Resource.PreApproval;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace PartyPic.ThirdParty.Impl
{
    public class MercadoPagoManager : IMercadoPagoManager
    {
        private readonly IConfiguration _config;

        public MercadoPagoManager(IConfiguration config)
        {
            _config = config;
        }

        public string CreateSubscription(MPSNewSubscriptionRequest request)
        {
            MercadoPagoConfig.AccessToken = _config.GetValue<string>("MercadoPagoSettings:AccessToken");

            try
            {
                var preapprovalClient = new PreapprovalClient();

                var preapprovalRequest = new PreapprovalCreateRequest
                {
                    Reason = "Suscripción a PartyPic",
                    ExternalReference = request.MarketReference.ToString(),
                    PayerEmail = request.UserEmail,
                    AutoRecurring = new PreApprovalAutoRecurringCreateRequest
                    {
                        Frequency = 1,
                        FrequencyType = request.PlanType,
                        TransactionAmount = (decimal)request.Amount,
                        CurrencyId = "ARS"
                    },
                    Status = "pending",
                    BackUrl = "https://local-web.partypic.com/subscripcion-existosa?externalRef=" + request.MarketReference.ToString()
                };

                Preapproval preapproval = preapprovalClient.Create(preapprovalRequest);

                return preapproval.InitPoint;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Preapproval GetSubscriptionByExternalReference(string externalReference)
        {
            MercadoPagoConfig.AccessToken = _config.GetValue<string>("MercadoPagoSettings:AccessToken");

            try
            {
                var preapprovalClient = new PreapprovalClient();

                var preapprovals = preapprovalClient.Search(new MercadoPago.Client.SearchRequest
                { 
                    Limit = 100,
                    Offset = 0,
                    Filters = new Dictionary<string, object>
                    {
                        { "external_reference", externalReference },
                        { "sort", "last_modified:desc"}
                    }
                });

                var subscription = preapprovals.Results.FirstOrDefault(p => p.ExternalReference.ToUpper() == externalReference.ToUpper());

                if (subscription == null)
                {
                    throw new KeyNotFoundException($"No se encontró una suscripción con la referencia externa '{externalReference}'.");
                }

                return subscription;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar la suscripción: {ex.Message}");
                throw;
            }
        }
    }
}
