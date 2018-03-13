using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tariffs.CommandLine
{
    public class CommandSpec
    {
        private readonly Func<CommandContext, Action<TextWriter, TextWriter>> _implementation;

        public CommandSpec(
            string name,
            Func<string, bool> selector,
            IReadOnlyCollection<string> parameters, 
            Func<CommandContext, Action<TextWriter, TextWriter>> implementation)
        {
            Name = name;
            Selector = selector;
            Parameters = parameters;
            _implementation = implementation;
            Docs = $"{name} {string.Join(" ", parameters.Select(p => $"<{p}>"))}";
        }

        public string Name { get; }

        public IReadOnlyCollection<string> Parameters { get; }

        public Func<string, bool> Selector { get; }

        public string Docs { get; }

        public Func<Action<TextWriter, TextWriter>> Build(CommandContext ctx) =>
            () => _implementation(ctx);

        public static CommandSpec Named(
            string name,
            IReadOnlyCollection<string> parameters,
            Func<CommandContext, Action<TextWriter, TextWriter>> implementation) => 
            new CommandSpec(
                name, 
                str => str.Equals(name, StringComparison.OrdinalIgnoreCase), 
                parameters,
                implementation);
    }
}