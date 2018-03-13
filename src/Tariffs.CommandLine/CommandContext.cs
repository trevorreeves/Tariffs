using System.Collections.Generic;
using System.Linq;

namespace Tariffs.CommandLine
{
    public class CommandContext
    {
        public CommandContext(IReadOnlyCollection<string> paramNames, IReadOnlyCollection<string> paramValues)
        {
            Parameters = paramNames
                .Zip(paramValues, (name, value) => (name, value))   
                .ToDictionary(kv => kv.Item1, kv => kv.Item2);
        }

        public IReadOnlyDictionary<string, string> Parameters { get; }
    }
}