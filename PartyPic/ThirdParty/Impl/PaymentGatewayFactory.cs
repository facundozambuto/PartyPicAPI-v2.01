using Microsoft.Extensions.Configuration;
using System;

namespace PartyPic.ThirdParty.Impl
{
    public class PaymentGatewayFactory
    {
        private readonly IConfiguration _configuration;

        public PaymentGatewayFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IPaymentGatewayStrategy GetPaymentGateway(string gateway)
        {
            return gateway switch
            {
                "MercadoPago" => new MercadoPagoManager(_configuration),
                "Mobbex" => new MobbexManager(),
                _ => throw new NotSupportedException($"Pasarela {gateway} no soportada")
            };
        }
    }
}
