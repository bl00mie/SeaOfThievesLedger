using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace LedgerScraper.Model
{
    class LedgerResponse
    {
        public Dictionary<string, LedgerEntity> Entries { get; set; } = new Dictionary<string, LedgerEntity>();
    }
}
