using System.Collections.Generic;
using System.IO;

namespace Tariffs.CommandLine
{
    public interface ICommandBootstrapper
    {
        void Execute(IReadOnlyCollection<string> args, TextWriter output, TextWriter errorOutput);

        void ExecuteInDebug(IReadOnlyCollection<string> args, TextWriter output, TextWriter errorOutput);
    }
}