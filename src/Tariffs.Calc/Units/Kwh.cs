using System;

namespace Tariffs.Calc.Units
{
    public sealed class Power { }

    public sealed class Gas { }

    public struct Kwh<TUnit>
    {
        public Kwh(decimal kwh)
        {
            if (kwh < 0) throw new ArgumentOutOfRangeException(nameof(kwh), "KWH must be greater than 0.");
            Value = kwh;
        }

        public decimal Value { get; }

        public override bool Equals(object obj) =>
            obj != null && obj is Kwh<TUnit> kwh && kwh.Value == Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static implicit operator decimal(Kwh<TUnit> kwh) => kwh.Value;
    }
}