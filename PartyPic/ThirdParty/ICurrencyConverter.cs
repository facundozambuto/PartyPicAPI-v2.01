using System.Threading.Tasks;

namespace PartyPic.ThirdParty
{
    public interface ICurrencyConverter
    {
        decimal GetAmountOfPesosByUSD(decimal amountOfUSD);
    }
}
