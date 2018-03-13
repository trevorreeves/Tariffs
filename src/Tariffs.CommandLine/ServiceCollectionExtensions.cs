using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Tariffs.CommandLine
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, Action<CommandConfigBuilder> configure)
        {
            services.TryAddSingleton<ICommandBootstrapper, CommandBootstrapper>();
            configure(new CommandConfigBuilder(services));

            return services;
        }

        public static void ExecuteCommands(this IServiceProvider provider, Action<ICommandBootstrapper> action)
        {
            action(provider.GetRequiredService<ICommandBootstrapper>());
        }

        public class CommandConfigBuilder
        {
            private readonly IServiceCollection _services;

            public CommandConfigBuilder(IServiceCollection services)
            {
                _services = services;
            }

            public CommandConfigBuilder AddCommand<TCommandDefinition>() where TCommandDefinition : class, ICommandDefinition
            {
                _services.AddSingleton<ICommandDefinition, TCommandDefinition>();
                return this;
            }
        }
    }
}