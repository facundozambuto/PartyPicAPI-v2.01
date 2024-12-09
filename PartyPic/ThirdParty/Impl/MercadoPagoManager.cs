using PartyPic.Models.Subscriptions;
using MercadoPago.Client.Preapproval;
using MercadoPago.Config;
using MercadoPago.Resource.PreApproval;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PartyPic.ThirdParty.Impl
{
    public class MercadoPagoManager : IMercadoPagoManager
    {
        private readonly IConfiguration _config;
        private const string BaseUrl = "https://api.mercadopago.com/preapproval/";

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

                var autoRecurring = new PreApprovalAutoRecurringCreateRequest
                {
                    Frequency = 1,
                    FrequencyType = request.PlanType,
                    TransactionAmount = (decimal)request.Amount,
                    CurrencyId = "ARS"
                };

                if (!request.IsAutoRenew)
                {
                    autoRecurring.StartDate = DateTime.Now;
                    autoRecurring.EndDate = request.PlanType == "months" ? DateTime.Now.AddMonths(1) : DateTime.Now.AddYears(1);
                }

                var preapprovalRequest = new PreapprovalCreateRequest
                {
                    Reason = "Suscripción a PartyPic",
                    ExternalReference = request.MarketReference.ToString(),
                    PayerEmail = request.UserEmail,
                    AutoRecurring = autoRecurring,
                    Status = "pending",
                    BackUrl = "http://local-web.partypic.com/admin/suscripcion-exitosa.html?externalRef=" + request.MarketReference.ToString()
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

        public async Task<MPSubscriptionResponse> GetSubscriptionAsync(string subscriptionId)
        {
            try
            {
                using var client = new HttpClient();

                var requestUrl = $"{BaseUrl}{subscriptionId}";
                
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.GetValue<string>("MercadoPagoSettings:AccessToken"));

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var subscriptionResponse = JsonConvert.DeserializeObject<MPSubscriptionResponse>(jsonResponse);
                
                return subscriptionResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
