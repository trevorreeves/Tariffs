using System;

namespace Tariffs.Calc
{
    public class TaxedValue
    {
        public TaxedValue(decimal preTax, decimal postTax)
        {
            PreTax = preTax;
            PostTax = postTax;
        }

        public decimal PreTax { get; }

        public decimal PostTax { get; }

        public static TaxedValue FromPreTaxValue(decimal pre, Func<decimal, decimal> applyTax) => 
            new TaxedValue(pre, applyTax(pre));

        public static TaxedValue FromPostTaxValue(decimal post, Func<decimal, decimal> removeTax) =>
            new TaxedValue(removeTax(post), post);
    }
}