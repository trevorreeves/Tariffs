using System;

namespace Tariffs.CommandLine
{
    public static class CommandContextExtensions
    {
        public static decimal GetRequiredDecimal(this CommandContext ctx, string name)
        {
            var res = ctx.GetDecimal(name, null);

            return res.valid && res.value.HasValue
                ? res.value.Value
                : throw new InvalidOperationException($"Missing a valid parameter value for {name}.");
        }

        public static string GetRequiredString(this CommandContext ctx, string name)
        {
            var res = ctx.GetString(name, Option<string>.None);

            return res.HasValue
                ? res.Value
                : throw new InvalidOperationException($"Missing a valid parameter value for {name}.");
        }

        public static TEnum GetRequiredEnum<TEnum>(this CommandContext ctx, string name) where TEnum : struct
        {
            var res = ctx.GetEnum<TEnum>(name, null);

            return res.valid && res.value.HasValue
                ? res.value.Value
                : throw new InvalidOperationException($"Missing a valid parameter value for {name}.");
        }

        public static Option<string> GetString(this CommandContext ctx, string name, Option<string> defaultValue) =>
            ctx.Parameters.ContainsKey(name) ?
                new Option<string>(ctx.Parameters[name]) :
                defaultValue;

        public static (bool valid, decimal? value) GetDecimal(this CommandContext ctx, string name, decimal? defaultValue) => 
            ctx.Parameters.ContainsKey(name) ? 
                (decimal.TryParse(ctx.Parameters[name], out var res) ? (true, res) : (false, new decimal?()))
                : (defaultValue.HasValue ? (true, defaultValue) : (true, new decimal?()));

        public static (bool valid, TEnum? value) GetEnum<TEnum>(this CommandContext ctx, string name, TEnum? defaultValue) where TEnum : struct =>
            ctx.Parameters.ContainsKey(name) ?
                (Enum.TryParse(ctx.Parameters[name], true, out TEnum res) ? (true, res) : (false, new TEnum?()))
                : (defaultValue.HasValue ? (true, defaultValue) : (true, new TEnum?()));
    }
}