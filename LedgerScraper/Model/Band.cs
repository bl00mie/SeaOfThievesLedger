using System;
using System.Collections.Generic;
using System.Text;

namespace LedgerScraper.Model
{
    public class Band
    {
        public int Index { get; set; }
        public IEnumerable<PlayerRank> Results { get; set; }
    }
}
