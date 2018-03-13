using Tariffs.Calc;
using Tariffs.CommandLine;
using Tariffs.Data;

namespace Tariffs.Console
{
    public class UsageCommand : ICommandDefinition
    {
        private enum FuelType
        {
            Gas,
            Power
        }

        public UsageCommand(ITariffSource source)
        {
            Spec = CommandSpec.Named(
                "usage",
                new[] {"TARIFF_NAME", "FUEL_TYPE", "TARGET_MONTHLY_SPEND"},
                ctx =>
                {
                    var tariffName = ctx.GetRequiredString("TARIFF_NAME");
                    var fuelType = ctx.GetRequiredEnum<FuelType>("FUEL_TYPE");
                    var monthlyBudget = TaxedValue.FromPostTaxValue(ctx.GetRequiredDecimal("TARGET_MONTHLY_SPEND"),
                        TaxHelper.RemoveTax);

                    if (!source.TryGet(tariffName, out var tariff))
                    {
                        return Commands.Error($"The specified tariff doesn't exist '{tariffName}'.");
                    }

                    // c# has no proper pattern matching, so I've run out of functional luck here...

                    switch (fuelType)
                    {
                        case FuelType.Gas:
                            if (!tariff.GasRate.HasValue)
                                return Commands.Error($"Tariff '{tariff.Name}' does not include {FuelType.Gas}.");

                            ctx.Output.WriteLine(FuelCalculator.AnnualGasUsage(tariff, monthlyBudget)
                                .GetValueOrDefault().Value); // just output 0 if tariff doesn't include fuel type
                            break;
                        case FuelType.Power:
                            if (!tariff.PowerRate.HasValue)
                                return Commands.Error($"Tariff '{tariff.Name}' does not include {FuelType.Power}.");

                            ctx.Output.WriteLine(FuelCalculator.AnnualPowerUsage(tariff, monthlyBudget)
                                .GetValueOrDefault().Value);
                            break;
                        default:
                            return Commands.Error($"Unsupported fuel type : {fuelType}");
                    }

                    return Commands.Ok();
                });
        }

        public CommandSpec Spec { get; }
    }
}