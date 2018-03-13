using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tariffs.Data
{
    public class TariffSource : ITariffSource
    {
        private IReadOnlyDictionary<string, Tariff> _tariffs = new Dictionary<string, Tariff>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="yieldFreshTariffs"></param>
        /// <returns></returns>
        /// <remarks>Very unsatisfactory to have a mutable dictionary reference in the params, but is 'best' way of staying in control
        /// of the equalility comparer at the moment.</remarks>
        public Task ReloadAsync(Func<Dictionary<string, Tariff>> yieldFreshTariffs)
        {
            return Task.Run(() =>
            {
                
                var newTariffs = new Dictionary<string, Tariff>(yieldFreshTariffs(), StringComparer.OrdinalIgnoreCase);

                // in a thread-safe, atomic manner, update the mutable state with our new tariff data
                Interlocked.Exchange(ref _tariffs, newTariffs);
            });
        }

        public IEnumerable<Tariff> Find(Func<Tariff, bool> predicate) => _tariffs.Values.Where(predicate);

        public IEnumerable<Tariff> GetAll() => _tariffs.Values;

        public bool TryGet(string name, out Tariff tariff) =>  _tariffs.TryGetValue(name, out tariff);
    }
}