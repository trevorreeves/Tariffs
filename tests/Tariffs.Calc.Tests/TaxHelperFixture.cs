using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Tariffs.Calc.Tests
{
    public class TaxHelperFixture
    {
        [Fact]
        public void ApplyTax_IncrementsValue()
        {
            TaxHelper.ApplyTax(100).Should().Be(105);
        }

        [Fact]
        public void RemoveTax_DecrementsValue()
        {
            TaxHelper.RemoveTax(105).Should().Be(100);
        }
    }
}
