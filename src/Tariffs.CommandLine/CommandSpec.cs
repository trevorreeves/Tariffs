using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tariffs.CommandLine
{
    public class CommandSpec
    {
        private readonly IReadOnlyCollection<string> _parameters;
        private readonly Func<CommandContext, TextWriter, Action<TextWriter, TextWriter>> _implementation;

        public CommandSpec(
            string name,
            Func<string, bool> selector,
            IReadOnlyCollection<string> parameters, 
            Func<CommandContext, TextWriter, Action<TextWriter, TextWriter>> implementation)
        {
            Name = name;
            Selector = selector;
            _parameters = parameters;
            _implementation = implementation;
            Docs = $"{name} {string.Join(" ", parameters.Select(p => $"<{p}>"))}";
        }

        public string Name { get; }

        public Func<string, bool> Selector { get; }

        public string Docs { get; }

        public Func<Action<TextWriter, TextWriter>> Build(IEnumerable<string> args, TextWriter output) =>
            () => _implementation(new CommandContext(_parameters, args.ToArray()), output);

        public static CommandSpec Named(
            string name,
            IReadOnlyCollection<string> parameters,
            Func<CommandContext, TextWriter, Action<TextWriter, TextWriter>> implementation) => 
            new CommandSpec(
                name, 
                str => str.Equals(name, StringComparison.OrdinalIgnoreCase), 
                parameters,
                implementation);
    }
}