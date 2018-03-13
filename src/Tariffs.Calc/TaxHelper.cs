namespace Tariffs.Calc
{
    public static class TaxHelper // TODO: interface ?
    {
        private const decimal VAT_RATE = 1.05M;

        public static decimal ApplyTax(decimal value) => value == 0 ? 0 : value * VAT_RATE;

        public static decimal RemoveTax(decimal value) => value == 0 ? 0 : value / VAT_RATE;
    }
}
