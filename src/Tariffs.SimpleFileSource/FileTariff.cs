using System;

namespace Tariffs.Data.SimpleFile
{
    public class FileTariff
    {
        public string tariff { get; set; }

        public FileTariffRate rates { get; set; }

        public decimal standing_charge { get; set; }
    }
}