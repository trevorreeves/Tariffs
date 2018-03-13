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
            var provider = new ServiceCollection()
                .AddSingleton(_ =>
                {
                    ITariffSource source = new TariffSource();
                    source.ReloadAsync(() => SimpleFileTariffLoader.Load(new FileInfo(Path.GetFullPath("./prices.json")))).Wait();
                    return source;
                })
                .AddCommands(config => config
                    .AddCommand<CostCommand>()
                    .AddCommand<UsageCommand>())
                .BuildServiceProvider();

            provider.ExecuteCommands(bootstrapper => 
                bootstrapper.Execute(args, System.Console.Out, System.Console.Error));
        }
    }
}
