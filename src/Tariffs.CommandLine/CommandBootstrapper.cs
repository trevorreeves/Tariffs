using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tariffs.CommandLine
{
    public class CommandBootstrapper : ICommandBootstrapper
    {
        private readonly IReadOnlyCollection<CommandSpec> _commandSpecs;

        public CommandBootstrapper(IEnumerable<ICommandDefinition> commandDefs)
        {
            _commandSpecs = commandDefs.Select(c => c.Spec).ToArray();
        }

        public void Execute(
            IReadOnlyCollection<string> args,
            TextWriter output,
            TextWriter errorOutput) =>
            Commands.Execute(_commandSpecs, args, output, errorOutput);

        public void ExecuteInDebug(
            IReadOnlyCollection<string> args,
            TextWriter output,
            TextWriter errorOutput) =>
            Commands.ExecuteInDebug(_commandSpecs, args, output, errorOutput);
    }
}