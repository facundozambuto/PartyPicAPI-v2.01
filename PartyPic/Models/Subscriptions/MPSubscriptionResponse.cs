using Newtonsoft.Json;
using System;

namespace PartyPic.Models.Subscriptions
{
    public class MPSubscriptionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("payer_id")]
        public int PayerId { get; set; }

        [JsonProperty("payer_email")]
        public string PayerEmail { get; set; }

        [JsonProperty("back_url")]
        public string BackUrl { get; set; }

        [JsonProperty("collector_id")]
        public int CollectorId { get; set; }

        [JsonProperty("application_id")]
        public long ApplicationId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("external_reference")]
        public string ExternalReference { get; set; }

        [JsonProperty("date_created")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("last_modified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("init_point")]
        public string InitPoint { get; set; }

        [JsonProperty("auto_recurring")]
        public AutoRecurring AutoRecurring { get; set; }

        [JsonProperty("summarized")]
        public Summarized Summarized { get; set; }

        [JsonProperty("next_payment_date")]
        public DateTime NextPaymentDate { get; set; }

        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonProperty("payment_method_id_secondary")]
        public object PaymentMethodIdSecondary { get; set; }

        [JsonProperty("first_invoice_offset")]
        public object FirstInvoiceOffset { get; set; }

        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }
    }

    public class AutoRecurring
    {
        [JsonProperty("frequency")]
        public int Frequency { get; set; }

        [JsonProperty("frequency_type")]
        public string FrequencyType { get; set; }

        [JsonProperty("transaction_amount")]
        public double TransactionAmount { get; set; }

        [JsonProperty("currency_id")]
        public string CurrencyId { get; set; }

        [JsonProperty("free_trial")]
        public object FreeTrial { get; set; }
    }

    public class Summarized
    {
        [JsonProperty("quotas")]
        public object Quotas { get; set; }

        [JsonProperty("charged_quantity")]
        public object ChargedQuantity { get; set; }

        [JsonProperty("pending_charge_quantity")]
        public object PendingChargeQuantity { get; set; }

        [JsonProperty("charged_amount")]
        public object ChargedAmount { get; set; }

        [JsonProperty("pending_charge_amount")]
        public object PendingChargeAmount { get; set; }

        [JsonProperty("semaphore")]
        public object Semaphore { get; set; }

        [JsonProperty("last_charged_date")]
        public object LastChargedDate { get; set; }

        [JsonProperty("last_charged_amount")]
        public object LastChargedAmount { get; set; }
    }
}