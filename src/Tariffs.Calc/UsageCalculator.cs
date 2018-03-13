using System;
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
                .Select(tariff => CalculateCost(tariff, powerUsage, gasUsage))
                .OrderBy(costs => costs.Total.PostTax);
        }

        private static UsageCost CalculateCost(Tariff tariff, Kwh<Power> powerUsage, Kwh<Gas> gasUsage)
        {
            var gas = tariff.GasRate * gasUsage ?? 0;
            var power = tariff.PowerRate * powerUsage ?? 0;

            var total = TaxedValue.FromPreTaxValue(gas + power + tariff.StandingCharge * 2, TaxHelper.ApplyTax);

            return new UsageCost(tariff, gas, power, total);
        }

        public static Kwh<Gas>? AnnualGasUsageFor(Tariff tariff, TaxedValue monthlyBudget)
        {
            return !tariff.GasRate.HasValue
                ? new Kwh<Gas>?()
                : new Kwh<Gas>((monthlyBudget.PreTax - tariff.StandingCharge) * 12 * tariff.GasRate.Value);
        }

        public static Kwh<Power>? AnnualPowerUsageFor(Tariff tariff, TaxedValue monthlyBudget)
        {
            return !tariff.PowerRate.HasValue
                ? new Kwh<Power>?()
                : new Kwh<Power>((monthlyBudget.PreTax - tariff.StandingCharge) * 12 * tariff.PowerRate.Value);
        }
    }
}
