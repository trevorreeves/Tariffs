using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
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
            // use only a little bit of DI to get everything started.
            var provider = new ServiceCollection()
                // register the tariff source, which is consumed by the commands
                .AddSingleton(_ =>
                {
                    // ensure the tarrif data is loaded before we use it.
                    ITariffSource source = new TariffSource();
                    source.ReloadAsync(() => SimpleFileTariffLoader.Load(new FileInfo(Path.GetFullPath("./prices.json")))).Wait();
                    return source;
                })
                // Register our little 'command line' component
                .AddCommands(config => config
                    // register the commands that we've implemented
                    .AddCommand<CostCommand>()
                    .AddCommand<UsageCommand>())
                // this builds the MS container
                .BuildServiceProvider();

            // run the app
            provider.ExecuteCommands(bootstrapper => 
                bootstrapper.Execute(args, System.Console.Out, System.Console.Error));
        }
    }
}
