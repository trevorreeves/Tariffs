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
        public void ApplyTax_To0_Returns0()
        {
            TaxHelper.ApplyTax(0).Should().Be(0);
        }

        [Fact]
        public void RemoveTax_DecrementsValue()
        {
            TaxHelper.RemoveTax(105).Should().Be(100);
        }

        [Fact]
        public void RemoveTax_From0_Returns0()
        {
            TaxHelper.RemoveTax(0).Should().Be(0);
        }
    }
}
