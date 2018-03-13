using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Tariffs.Data.Tests
{
    public class TariffSourceFixture
    {
        private readonly TariffSource _source;

        public TariffSourceFixture()
        {
            _source = new TariffSource();
        }
        
        [Fact]
        public async Task TryGet_TariffExists_ReturnsTariff()
        {
            // arrange
            var expected = new Tariff("two", 4.0M, 5.0M, 6.0M);
            await _source.ReloadAsync(() => new []
            {
                new Tariff("one", 1.0M, 2.0M, 3.0M),
                expected
            });

            // act
            _source.TryGet("two", out var actual).Should().Be(true);

            // assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task TryGet_TariffExistsWithDifferentCasing_ReturnsTariff()
        {
            // arrange
            var expected = new Tariff("two", 4.0M, 5.0M, 6.0M);
            await _source.ReloadAsync(() => new []
            {
                new Tariff("one", 1.0M, 2.0M, 3.0M),
                expected
            });

            // act
            _source.TryGet("TWO", out var actual).Should().Be(true);

            // assert
            actual.Should().Be(expected);
        }

        [Fact]
        public async Task TryGet_TariffDoesntExist_ReturnsFalse()
        {
            // arrange
            await _source.ReloadAsync(() => new []
            {
                new Tariff("one", 1.0M, 2.0M, 3.0M)
            });

            // act
            _source.TryGet("two", out var actual).Should().Be(false);

            // assert
            actual.Should().BeNull();
        }

        [Fact]
        public async Task GetAll_TariffsExist_ReturnsAll()
        {
            // arrange
            var one = new Tariff("one", 1.0M, 2.0M, 3.0M);
            var two = new Tariff("two", 4.0M, 5.0M, 6.0M);
            await _source.ReloadAsync(() => new []
            {
                one,
                two
            });

            // act
            var actual = _source.GetAll();

            // assert
            actual.Should().BeEquivalentTo(new[] {one, two});
        }

        [Fact]
        public async Task GetAll_NoTariffs_ReturnsEmpty()
        {
            // arrange
            await _source.ReloadAsync(() => new Tariff[0]);

            // act
            var actual = _source.GetAll();

            // assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task Find_TariffMatches_ReturnsTariff()
        {
            // arrange
            var expected = new Tariff("two", 4.0M, 5.0M, 6.0M);
            await _source.ReloadAsync(() => new []
            {
                new Tariff("one", 1.0M, 2.0M, 3.0M),
                expected
            });

            // act
            var actual = _source.Find(t => t.PowerRate == 4.0M).ToArray();

            // assert
            actual.Single().Should().Be(expected);
        }

        [Fact]
        public async Task Find_NoTariffMatches_ReturnsNone()
        {
            // arrange
            var expected = new Tariff("two", 4.0M, 5.0M, 6.0M);
            await _source.ReloadAsync(() => new []
            {
                new Tariff("one", 1.0M, 2.0M, 3.0M),
                expected
            });

            // act
            var actual = _source.Find(t => t.PowerRate == 99999M).ToArray();

            // assert
            actual.Should().BeEmpty();
        }

        [Fact]
        public async Task Reload_BruteForceAttack_IsThreadSafe()
        {
            // run the test for 10 seconds
            var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            // run a set of concurrent searches continually on the source
            var searchTasks = Enumerable
                .Range(1, 10)
                .Select(i => Task.Run(() =>
                {
                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        _source.Find(_ => true);
                    }
                }));

            // run reloads continually on the source
            var reloadTask = Task.Run(() =>
                {
                    var rand = new Random();

                    while (!cancellation.Token.IsCancellationRequested)
                    {
                        _source.ReloadAsync(() =>
                            Enumerable
                                .Range(1, rand.Next(5, 100))
                                .Select(j => new Tariff(j.ToString(), j * 2, j * 3, j * 4)));
                    }
                });

            // all tasks should complete without error.
            await Task.WhenAll(searchTasks.Concat(new[] { reloadTask }));
        }
    }
}
