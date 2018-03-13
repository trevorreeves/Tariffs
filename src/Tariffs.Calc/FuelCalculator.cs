using System;
using System.Collections.Generic;
using System.Linq;
using Tariffs.Calc.Units;
using Tariffs.Data;

namespace Tariffs.Calc
{
    public static class FuelCalculator
    {
        public static IEnumerable<UsageCost> CostsPerTariff(IEnumerable<Tariff> tariffs, Kwh<Power> powerUsage, Kwh<Gas> gasUsage) => 
            tariffs
                .Select(tariff => CostForTariff(tariff, powerUsage, gasUsage))
                .OrderBy(costs => costs.Total.PostTax);

        private static UsageCost CostForTariff(Tariff tariff, Kwh<Power> powerUsage, Kwh<Gas> gasUsage)
        {
            decimal? ApplyUsage<T>(decimal? input, Kwh<T> usage) => 
                input * usage ?? input;

            decimal? ApplyStandingCharge(decimal? input, decimal charge) =>
                input + charge*12 ?? input;

            // ok, so this would look prettier with a function pipelining operator..
            var g = ApplyStandingCharge(ApplyUsage(tariff.GasRate, gasUsage), tariff.StandingCharge);
            var p = ApplyStandingCharge(ApplyUsage(tariff.PowerRate, powerUsage), tariff.StandingCharge);

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
