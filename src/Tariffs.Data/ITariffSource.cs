using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tariffs.Data
{
    public interface ITariffSource
    {
        Task ReloadAsync(Func<IEnumerable<Tariff>> yieldFreshTariffs);

        IEnumerable<Tariff> Find(Func<Tariff, bool> predicate);

        IEnumerable<Tariff> GetAll();

        bool TryGet(string name, out Tariff tariff);
    }
}
