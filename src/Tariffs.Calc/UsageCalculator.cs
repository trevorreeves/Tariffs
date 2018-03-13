using System.Collections.Generic;
using System.Linq;
using Tariffs.Calc.Units;
using Tariffs.Data;

namespace Tariffs.Calc
{
    public static class UsageCalculator
    {
        public static IEnumerable<UsageCost> CostsPerTariffFor(IEnumerable<Tariff> tariffs, Kwh<Power> powerUsage, Kwh<Gas> gasUsage)
        {
            return tariffs
                .Select(tariff =>
                { // TODO: seperate into seperate function
                    var gas = tariff.GasRate * gasUsage;
                    var power = tariff.PowerRate * powerUsage;
                    var total = TaxedValue.FromPreTaxValue(gas + power + tariff.StandingCharge * 2, TaxHelper.ApplyTax);

                    return new UsageCost(tariff, gas, power, total);
                })
                .OrderBy(costs => costs.Total);
        }

        public static Kwh<Gas> AnnualGasUsageFor(Tariff tariff, TaxedValue monthlyBudget)
        {
            // TODO: no rate defined in tariff
            var annualKwhBudget = (monthlyBudget.PreTax - tariff.StandingCharge) * 12;
            return new Kwh<Gas>(annualKwhBudget * tariff.GasRate);
        }

        public static Kwh<Power> AnnualPowerUsageFor(Tariff tariff, TaxedValue monthlyBudget)
        {
            // TODO: no rate defined in tariff
            var annualKwhBudget = (monthlyBudget.PreTax - tariff.StandingCharge) * 12;
            return new Kwh<Power>(annualKwhBudget * tariff.PowerRate);
        }
    }
}
