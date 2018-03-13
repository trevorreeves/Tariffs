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
                    var powerKwh = new Kwh<Power>(ctx.GetRequiredDecimal("POWER_USAGE_KWH"));
                    var gasKwh = new Kwh<Gas>(ctx.GetRequiredDecimal("GAS_USAGE_KWH"));

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