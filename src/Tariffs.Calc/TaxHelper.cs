namespace Tariffs.Calc
{
    public static class TaxHelper // TODO: interface ?
    {
        private const decimal VAT_RATE = 0.05M;

        public static decimal ApplyTax(decimal value) => value * VAT_RATE;

        public static decimal RemoveTax(decimal value) => (value / (VAT_RATE * 100 + 100)) * 100;
    }
}
