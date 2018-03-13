﻿using System;
using System.IO;
using System.Linq;
using Tariffs.Calc;
using Tariffs.Calc.Units;
using Tariffs.CommandLine;
using Tariffs.Data;
using Tariffs.Data.SimpleFile;

namespace Tariffs.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ITariffSource source = new TariffSource();
            source.ReloadAsync(() => SimpleFileTariffLoader.Load(new FileInfo(Path.GetFullPath("./prices.json")))).Wait();

            // TODO: Tariffs will only ever be a small data set (10s not 1000s), so we'll always want to keep it in memory.
            // But they might be modified during the lifetime of the app (if it becomes long running), so support reloading data...
            // new SimpleFileWatcher(file)
            // watcher.Changes.Subscribe(ctx => source.Reload().Wait());

            var commands = new[]
            {
                CommandSpec.Named(
                    "cost",
                    new[] {"POWER_USAGE_KWH", "GAS_USAGE_KWH"},
                    ctx =>
                    {
                        var powerKwh = new Kwh<Power>(ctx.GetRequiredDecimal("POWER_USAGE_KWH"));
                        var gasKwh = new Kwh<Gas>(ctx.GetRequiredDecimal("GAS_USAGE_KWH"));

                        foreach (var line in UsageCalculator
                            .CostsPerTariffFor(source.GetAll(), powerKwh, gasKwh)
                            .Select(cost => $"{cost.Tariff.Name} {cost.Total.PostTax:F2}"))
                        {
                            ctx.Output.WriteLine(line);
                        }

                        return Commands.Ok();
                    }),
                CommandSpec.Named(
                    "usage",
                    new[] {"TARIFF_NAME", "FUEL_TYPE", "TARGET_MONTHLY_SPEND"},
                    ctx =>
                    {
                        var tariffName = ctx.GetRequiredString("TARIFF_NAME");
                        var fuelType = ctx.GetRequiredEnum<FuelType>("FUEL_TYPE");
                        var monthlyBudget = TaxedValue.FromPostTaxValue(ctx.GetRequiredDecimal("TARGET_MONTHLY_SPEND"), TaxHelper.RemoveTax);

                        if (!source.TryGet(tariffName, out var tariff))
                        {
                            return Commands.Error($"The specified tariff doesn't exist '{tariffName}'.");
                        }

                        switch (fuelType)
                        {
                            case FuelType.Gas:
                                ctx.Output.WriteLine(UsageCalculator.AnnualGasUsageFor(tariff, monthlyBudget).Value);
                                break;
                            case FuelType.Power:
                                ctx.Output.WriteLine(UsageCalculator.AnnualPowerUsageFor(tariff, monthlyBudget).Value);
                                break;
                            default:
                                return Commands.Error($"Unsupported fuel type : {fuelType}");
                        }

                        return Commands.Ok();
                    }),
            };

            Commands.Execute(commands, args, System.Console.Out, System.Console.Error);
        }

        private enum FuelType
        {
            Gas,
            Power
        }
    }
}
