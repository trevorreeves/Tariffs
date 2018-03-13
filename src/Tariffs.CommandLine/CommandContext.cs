using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tariffs.CommandLine
{
    public class CommandContext
    {
        public CommandContext(
            string commandName, 
            IReadOnlyCollection<string> paramNames, 
            IReadOnlyCollection<string> paramValues,
            TextWriter output)
        {
            CommandName = commandName;
            Output = output;
            Parameters = paramNames
                .Zip(paramValues, (name, value) => (name, value))   
                .ToDictionary(kv => kv.Item1, kv => kv.Item2);
        }

        public string CommandName { get; }

        public TextWriter Output { get; }

        public IReadOnlyDictionary<string, string> Parameters { get; }
    }
}