using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tariffs.CommandLine
{
    public static class Commands
    {
        public static void ShowHelp(IEnumerable<CommandSpec> commands, TextWriter output)
        {
            output.WriteLine("Available commands : ");
            output.WriteLine(string.Join(",\n", commands.Select(c => c.Docs)));
        }

        public static void ExecuteInDebug(
            IReadOnlyCollection<CommandSpec> commands,
            IReadOnlyCollection<string> args,
            TextWriter output,
            TextWriter errorOutput)
        {
            BuildImplementation(commands, args, output)
                .ShowExceptionDetail()
                .Dispatch(output, errorOutput);
        }

        public static void Execute(
            IReadOnlyCollection<CommandSpec> commands,
            IReadOnlyCollection<string> args,
            TextWriter output,
            TextWriter errorOutput)
        {
            BuildImplementation(commands, args, output)
                .HideExceptionDetail()
                .Dispatch(output, errorOutput);
        }

        private static Func<Action<TextWriter, TextWriter>> BuildImplementation(
            IReadOnlyCollection<CommandSpec> commands,
            IReadOnlyCollection<string> args,
            TextWriter output)
        {
            var commandName = args?.FirstOrDefault() ?? string.Empty;

            var command = commands
                .Concat(new[]
                {
                    new CommandSpec(
                        "No command fallback",
                        string.IsNullOrWhiteSpace,
                        new string[0],
                        ctx =>
                        {
                            return (info, error) =>
                            {
                                ShowHelp(commands, info);
                            };
                        }),
                    new CommandSpec(
                        "Unrecognised command fallback",
                        str => !string.IsNullOrWhiteSpace(str),
                        new string[0],
                        ctx =>
                        {
                            return (info, error) =>
                            {
                                error.WriteLine($"No valid command found for '{ctx.CommandName}'.");
                                ShowHelp(commands, info);
                            };
                        })
                })
                .First(c => c.Selector(commandName));

            return command.Build(new CommandContext(
                commandName, 
                command.Parameters, 
                args.Skip(1).ToArray(), 
                output));
        }

        private static Func<Action<TextWriter, TextWriter>> HideExceptionDetail(
            this Func<Action<TextWriter, TextWriter>> impl)
        {
            return () =>
            {
                try
                {
                    return impl();
                }
                catch (Exception e)
                {
                    return Error(e.Message); // will show only headline message of ex
                }
            };
        }

        private static Func<Action<TextWriter, TextWriter>> ShowExceptionDetail(
            this Func<Action<TextWriter, TextWriter>> impl)
        {
            return () =>
            {
                try
                {
                    return impl();
                }
                catch (Exception e)
                {
                    return Error(e.ToString()); // will expose stack trace
                }
            };
        }

        private static void Dispatch(
            this Func<Action<TextWriter, TextWriter>> impl,
            TextWriter output,
            TextWriter errorOutput)
        {
            impl()(output, errorOutput);
        }

        public static Action<TextWriter, TextWriter> Ok() => (_, __) => { };

        public static Action<TextWriter, TextWriter> Error(string message) =>
            (_, errorOutput) => errorOutput.WriteLine(message);
    }
}