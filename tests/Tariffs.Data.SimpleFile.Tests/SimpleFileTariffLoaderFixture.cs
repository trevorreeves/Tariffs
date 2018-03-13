using System;
using System.IO;
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

            //{ "tariff": "better-energy", "rates": { "power":  0.1367, "gas": 0.0288}, "standing_charge": 8.33},
            //{ "tariff": "2yr-fixed", "rates": { "power": 0.1397, "gas": 0.0296}, "standing_charge": 8.75},
            //{ "tariff": "greener-energy", "rates": { "power":  0.1544}, "standing_charge": 8.33},
            //{ "tariff": "simpler-energy", "rates": { "power":  0.1396, "gas": 0.0328}, "standing_charge": 8.75}

            actual.Should().HaveCount(4);

            actual.Keys.Should()
                .BeEquivalentTo(new[] {"better-energy", "2yr-fixed", "greener-energy", "simpler-energy"});

            var better = actual["better-energy"];
            better.Name.Should().Be("better-energy");
            better.GasRate.Should().Be(0.0288M);
            better.PowerRate.Should().Be(0.1367M);
            better.StandingCharge.Should().Be(8.33M);

            var greener = actual["greener-energy"];
            greener.Name.Should().Be("greener-energy");
            greener.GasRate.Should().Be(0M);
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
