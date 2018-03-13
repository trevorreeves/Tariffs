using System;
using System.Collections.Generic;

namespace Tariffs.Data
{
    public interface ITariffSource
    {
        IEnumerable<Tariff> Find(Func<Tariff, bool> predicate);
    }
}
