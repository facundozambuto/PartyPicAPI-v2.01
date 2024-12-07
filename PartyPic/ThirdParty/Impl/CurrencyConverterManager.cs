using Newtonsoft.Json;
using PartyPic.Models.Common;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PartyPic.ThirdParty.Impl
{
    public class CurrencyConverterManager : ICurrencyConverter
    {
        public decimal GetAmountOfPesosByUSD(decimal amountOfUSD)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = Task.Run(() => client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://dolarapi.com/v1/dolares/oficial"))).Result;

                        response.EnsureSuccessStatusCode();

                        var result = Task.Run(() => response.Content.ReadAsStringAsync()).Result;

                        var currencyQuotation = JsonConvert.DeserializeObject<CurrencyQuotationResponse>(result);

                        var averageExchangeRate = (currencyQuotation.Compra + currencyQuotation.Venta) / 2;

                        var amountOfPesos = amountOfUSD * averageExchangeRate;

                        return amountOfPesos;
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
}
