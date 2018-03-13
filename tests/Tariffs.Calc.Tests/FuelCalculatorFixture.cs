using System;
using System.Linq;
using FluentAssertions;
using Tariffs.Calc.Units;
using Tariffs.Data;
using Xunit;

namespace Tariffs.Calc.Tests
{
    public class FuelCalculatorFixture
    {
        [Fact]
        public void CostsPerTariff_AllFuelTypesSupported_ReturnsCostsInOrder()
        {
            // Arrange
            var tariff1 = new Tariff("test1", 2m, 3m, 4m);
            var tariff2 = new Tariff("test2", 4m, 6m, 8m);

            // Act
            var actual = FuelCalculator.CostsPerTariff(new[] {tariff1, tariff2}, new Kwh<Power>(1000), new Kwh<Gas>(2000)).ToList();

            // Assert
            actual.Should().HaveCount(2);
            var actualCost = actual.First();
            actualCost.Tariff.Should().Be(tariff1);
            actualCost.Total.PostTax.Should().Be(((4 * 12) + (1000 * 2) + (4 * 12) + (2000 * 3)) * 1.05M);

            actual.ElementAt(1).Tariff.Should().Be(tariff2);
        }

        [Fact]
        public void CostsPerTariff_GasNotInTariff_ReturnsCosts()
        {
            // Arrange
            var tariff = new Tariff("test", 2m, null, 4m);

            // Act
            var actual = FuelCalculator.CostsPerTariff(new[] { tariff }, new Kwh<Power>(1000), new Kwh<Gas>(2000)).ToList();

            // Assert
            actual.Should().HaveCount(1);
            var actualCost = actual.First();
            actualCost.Tariff.Should().Be(tariff);
            actualCost.Total.PostTax.Should().Be(((4 * 12) + (1000 * 2)) * 1.05M);
        }

        [Fact]
        public void CostsPerTariff_PowerNotInTariff_ReturnsCosts()
        {
            // Arrange
            var tariff = new Tariff("test", null, 3m, 4m);

            // Act
            var actual = FuelCalculator.CostsPerTariff(new[] { tariff }, new Kwh<Power>(1000), new Kwh<Gas>(2000)).ToList();

            // Assert
            actual.Should().HaveCount(1);
            var actualCost = actual.First();
            actualCost.Tariff.Should().Be(tariff);
            actualCost.Total.PostTax.Should().Be(((4 * 12) + (2000 * 3)) * 1.05M);
        }

        [Fact]
        public void AnnualGasUsage_TariffWithGas_ReturnsUsage()
        {
            // Arrange
            var tariff = new Tariff("test", null, 2m, 3m);

            // Act
            var actual = FuelCalculator.AnnualGasUsage(tariff, TaxedValue.FromPreTaxValue(100, x => x));

            // Assert
            actual.HasValue.Should().BeTrue();
            actual.Value.Value.Should().Be((100 - 3)*12 * 2);
        }

        [Fact]
        public void AnnualGasUsage_TariffWithNoGas_ReturnsNothing()
        {
            // Arrange
            var tariff = new Tariff("test", null, null, 3m);

            // Act
            var actual = FuelCalculator.AnnualGasUsage(tariff, TaxedValue.FromPreTaxValue(100, x => x));

            // Assert
            actual.HasValue.Should().BeFalse();
        }

        [Fact]
        public void AnnualPowerUsage_TariffWithGas_ReturnsUsage()
        {
            // Arrange
            var tariff = new Tariff("test", 2m, null, 3m);

            // Act
            var actual = FuelCalculator.AnnualPowerUsage(tariff, TaxedValue.FromPreTaxValue(100, x => x));

            // Assert
            actual.HasValue.Should().BeTrue();
            actual.Value.Value.Should().Be((100 - 3) * 12 * 2);
        }

        [Fact]
        public void AnnualPowerUsage_TariffWithNoGas_ReturnsNothing()
        {
            // Arrange
            var tariff = new Tariff("test", null, null, 3m);

            // Act
            var actual = FuelCalculator.AnnualPowerUsage(tariff, TaxedValue.FromPreTaxValue(100, x => x));

            // Assert
            actual.HasValue.Should().BeFalse();
        }
    }
}
