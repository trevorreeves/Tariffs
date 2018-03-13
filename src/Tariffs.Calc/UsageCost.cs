using Tariffs.Data;

namespace Tariffs.Calc
{
    public class UsageCost 
    {
        public UsageCost(Tariff tariff, decimal gasPrice, decimal powerPrice, TaxedValue total)
        {
            GasPrice = gasPrice;
            PowerPrice = powerPrice;
            Tariff = tariff;
            Total = total;
        }

        public decimal GasPrice { get; }

        public decimal PowerPrice { get; }

        public Tariff Tariff { get; }

        public TaxedValue Total { get; }
    }
}
