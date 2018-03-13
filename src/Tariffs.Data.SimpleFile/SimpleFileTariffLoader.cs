using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Tariffs.Data.SimpleFile
{
    public static class SimpleFileTariffLoader
    {
        public static IEnumerable<Tariff> Load(FileInfo sourceFile)
        {
            if (!sourceFile.Exists) throw new ArgumentException(nameof(sourceFile), $"Tariff source file does not exist : {sourceFile.FullName}.");

            var fileContents = File.ReadAllText(sourceFile.FullName);

            return string.IsNullOrWhiteSpace(fileContents) ?
                Enumerable.Empty<Tariff>():
                JsonConvert.DeserializeObject<List<FileTariff>>(fileContents)
                    .Select(MapToDataModel);
        }

        private static Tariff MapToDataModel(FileTariff fileTariff)
        {
            return new Tariff(
                fileTariff.tariff,
                fileTariff.rates.power,
                fileTariff.rates.gas,
                fileTariff.standing_charge);
        }
    }
}
