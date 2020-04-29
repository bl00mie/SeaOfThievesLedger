using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace LedgerScraper.Model
{
    class EntitiesResponse<T> where T : TableEntity
    {
        public Dictionary<string, T> Entries { get; set; } = new Dictionary<string, T>();
    }
}
