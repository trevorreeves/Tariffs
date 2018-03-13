using System.Linq;
using Tariffs.Calc;
using Tariffs.Calc.Units;
using Tariffs.CommandLine;
using Tariffs.Data;

namespace Tariffs.Console
{
    public class CostCommand : ICommandDefinition
    {
        public CostCommand(ITariffSource source)
        {
            Spec = CommandSpec.Named(
                "cost",
                new[] {"POWER_USAGE_KWH", "GAS_USAGE_KWH"},
                ctx =>
                {
                    var p = ctx.GetRequiredDecimal("POWER_USAGE_KWH");
                    var powerKwh = p == 0 ? new Kwh<Power>?() : new Kwh<Power>(p);

                    var g = ctx.GetRequiredDecimal("GAS_USAGE_KWH");
                    var gasKwh = g == 0 ? new Kwh<Gas>?() : new Kwh<Gas>(g);

                    foreach (var line in FuelCalculator
                        .CostsPerTariff(source.GetAll(), powerKwh, gasKwh)
                        .Select(cost => $"{cost.Tariff.Name} {cost.Total.PostTax:F2}"))
                    {
                        ctx.Output.WriteLine(line);
                    }

                    return Commands.Ok();
                });
        }

        public CommandSpec Spec { get; }
    }
}