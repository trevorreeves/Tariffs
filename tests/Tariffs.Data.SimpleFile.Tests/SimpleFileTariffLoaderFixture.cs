using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Tariffs.Data.SimpleFile.Tests
{
    public class SimpleFileTariffLoaderFixture
    {
        [Fact]
        public void Load_ValidFileExists_LoadsDataCorrectly()
        {
            var actual = SimpleFileTariffLoader.Load(new FileInfo("./data/prices.json"));

            var actualLookup = actual.ToLookup(t => t.Name);
            actualLookup.Should().HaveCount(4);

            actualLookup["2yr-fixed"].Should().HaveCount(1);
            actualLookup["simpler-energy"].Should().HaveCount(1);

            var better = actualLookup["better-energy"].Single();
            better.Name.Should().Be("better-energy");
            better.GasRate.Should().Be(0.0288M);
            better.PowerRate.Should().Be(0.1367M);
            better.StandingCharge.Should().Be(8.33M);

            var greener = actualLookup["greener-energy"].Single();
            greener.Name.Should().Be("greener-energy");
            greener.GasRate.Should().BeNull();
            greener.PowerRate.Should().Be(0.1544M);
            greener.StandingCharge.Should().Be(8.33M);
        }

        [Fact]
        public void Load_EmptyFileExists_LoadsEmptyData()
        {
            var actual = SimpleFileTariffLoader.Load(new FileInfo("./data/empty-prices.json"));

            actual.Should().BeEmpty();
        }

        [Fact]
        public void Load_NoFileExists_Throws()
        {
            new Action(() => SimpleFileTariffLoader.Load(new FileInfo("./idontexist")))
                .Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Load_InvalidJsonFileExists_Throws()
        {
            new Action(() => SimpleFileTariffLoader.Load(new FileInfo("./invalid-prices.json")))
                .Should().Throw<ArgumentException>();
        }
    }
}
