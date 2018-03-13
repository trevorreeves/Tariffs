namespace Tariffs.Data
{
    public class Tariff
    {
        public Tariff(string name, decimal powerRate, decimal gasRate, decimal standingCharge)
        {
            Name = name;
            PowerRate = powerRate;
            GasRate = gasRate;
            StandingCharge = standingCharge;
        }

        public string Name { get; }

        // TODO: no rate defined in tariff - just leave as 0, or model explicitly?
        public decimal PowerRate { get; }

        // TODO: no gas rate in tariff, but gas rate usage supplied
        public decimal GasRate { get; }

        public decimal StandingCharge { get; }
    }   
}
