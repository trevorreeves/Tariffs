using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tariffs.Data.SimpleFile
{
    // TODO: move to own project
    // TODO: add file watcher and reload in a thread safe way

    public class SimpleFileTariffSource : ITariffSource
    {
        public SimpleFileTariffSource(FileInfo sourceFile)
        {
            
        }

        public Task Reload()
        {
            return Task.CompletedTask;
        }

        public IEnumerable<Tariff> Find(Func<Tariff, bool> predicate)
        {
            // TODO
            /*
                - load json into file model
                - map file model to data model
             */
            throw new NotImplementedException();
        }
    }
}
