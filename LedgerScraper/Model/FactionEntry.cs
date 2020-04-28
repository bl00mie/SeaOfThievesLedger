using System;
using System.Collections.Generic;
using System.Text;

namespace LedgerScraper.Model
{
    public class FactionData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IEnumerable<Band> Bands { get; set; }
        public bool error { get; set; }

    }
}
