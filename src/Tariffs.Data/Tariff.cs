namespace Tariffs.Data
{
    public class Tariff
    {
        public Tariff(string name, decimal? powerRate, decimal? gasRate, decimal standingCharge)
        {
            Name = name;
            PowerRate = powerRate;
            GasRate = gasRate;
            StandingCharge = standingCharge;
        }

        public string Name { get; }

        public decimal? PowerRate { get; }

        public decimal? GasRate { get; }

        public decimal StandingCharge { get; }
    }   
}
