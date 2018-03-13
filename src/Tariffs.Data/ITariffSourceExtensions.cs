using System;
using System.Collections.Generic;
using System.Linq;

namespace Tariffs.Data
{
    public static class ITariffSourceExtensions
    {
        public static IEnumerable<Tariff> GetAll(this ITariffSource source) => source.Find(_ => true);

        public static bool TryGet(this ITariffSource source, string name, out Tariff tariff)
        {
            tariff =  source
                .Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            return tariff != default(Tariff);
        }
    }
}
