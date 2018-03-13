using System;
using System.Linq;
using Tariffs.Calc;
using Tariffs.Calc.Units;
using Tariffs.CommandLine;
using Tariffs.Data;
using Tariffs.Data.SimpleFile;

namespace Tariffs.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tariffs will only ever be a small data set (10s not 1000s), so we'll always want to keep it in memory.
            // But they might be modified during the lifetime of the app, so support reloading data.

            var _source = new SimpleFileTariffSource(null);

            // new SimpleFileWatcher(file)
            // watcher.Changes.Subscribe(ctx => source.Reload().Wait());

            var commands = new[]
            {
                CommandSpec.Named(
                    "cost",
                    new[] {"POWER_USAGE_KWH", "GAS_USAGE_KWH"},
                    (ctx, writer) =>
                    {
                        var powerKwh = new Kwh<Power>(ctx.GetRequiredDecimal("POWER_USAGE_KWH"));
                        var gasKwh = new Kwh<Gas>(ctx.GetRequiredDecimal("GAS_USAGE_KWH"));

                        foreach (var line in UsageCalculator
                            .CostsPerTariffFor(_source.GetAll(), powerKwh, gasKwh)
                            .Select(cost => $"{cost.Tariff} {cost.Total.PostTax}"))
                        {
                            writer.WriteLine(line);
                        }

                        return Commands.Ok();
                    }),
                CommandSpec.Named(
                    "usage",
                    new[] {"TARIFF_NAME", "FUEL_TYPE", "TARGET_MONTHLY_SPEND"},
                    (ctx, writer) =>
                    {
                        var tariffName = ctx.GetRequiredString("TARIFF_NAME");
                        var fuelType = ctx.GetRequiredEnum<FuelType>("FUEL_TYPE");
                        var monthlyBudget = TaxedValue.FromPostTaxValue(ctx.GetRequiredDecimal("TARGET_MONTHLY_SPEND"), TaxHelper.RemoveTax);

                        if (!_source.TryGet(tariffName, out var tariff))
                        {
                            return Commands.Error($"The specified tariff doesn't exist '{tariffName}'.");
                        }

                        switch (fuelType)
                        {
                            case FuelType.Gas:
                                writer.WriteLine(UsageCalculator.AnnualGasUsageFor(tariff, monthlyBudget).Value);
                                break;
                            case FuelType.Power:
                                writer.WriteLine(UsageCalculator.AnnualPowerUsageFor(tariff, monthlyBudget).Value);
                                break;
                            default:
                                return Commands.Error($"Unsupported fuel type : {fuelType}");
                        }

                        return Commands.Ok();
                    }),
            };

            Commands.ShowHelp(commands, System.Console.Out);
            Commands.Execute(commands, args, System.Console.Out, System.Console.Error);
        }

        public enum FuelType
        {
            Gas,
            Power
        }
    }
}
