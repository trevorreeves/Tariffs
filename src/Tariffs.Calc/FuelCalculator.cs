using System;
using System.Collections.Generic;
using System.Linq;
using Tariffs.Calc.Units;
using Tariffs.Data;

namespace Tariffs.Calc
{
    public static class FuelCalculator
    {
        public static IEnumerable<UsageCost> CostsPerTariff(IEnumerable<Tariff> tariffs, Kwh<Power>? powerUsage, Kwh<Gas>? gasUsage) => 
            // we take care as either the tariffs could only have partial fuel coverage, and or the customer usage might be partial
            tariffs
                .Where(t => ((powerUsage.HasValue && t.PowerRate.HasValue) || !powerUsage.HasValue) &&
                            ((gasUsage.HasValue && t.GasRate.HasValue) || !gasUsage.HasValue)) // only include compatible tariffs
                .Select(t => CostForTariff(t, powerUsage, gasUsage))
                .OrderBy(costs => costs.Total.PostTax);

        private static UsageCost CostForTariff(Tariff tariff, Kwh<Power>? powerUsage, Kwh<Gas>? gasUsage)
        {
            var g = tariff.GasRate * gasUsage + tariff.StandingCharge*12;
            var p = tariff.PowerRate * powerUsage + tariff.StandingCharge*12;

            var total = TaxedValue.FromPreTaxValue((g??0) + (p??0), TaxHelper.ApplyTax);

            return new UsageCost(tariff, g, p, total);
        }

        public static Kwh<Gas>? AnnualGasUsage(Tariff tariff, TaxedValue monthlyBudget) =>
            AnnualFuelUsage<Gas>(tariff.GasRate, tariff.StandingCharge, monthlyBudget);

        public static Kwh<Power>? AnnualPowerUsage(Tariff tariff, TaxedValue monthlyBudget) => 
            AnnualFuelUsage<Power>(tariff.PowerRate, tariff.StandingCharge, monthlyBudget);

        private static Kwh<TFuel>? AnnualFuelUsage<TFuel>(decimal? rate, decimal standingCharge, TaxedValue monthlyBudget) => 
            !rate.HasValue 
                ? new Kwh<TFuel>?()
                : new Kwh<TFuel>((monthlyBudget.PreTax - standingCharge) * 12 * rate.Value);
    }
}
