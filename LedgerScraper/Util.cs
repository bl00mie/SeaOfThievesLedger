using System;
using System.Collections.Generic;
using System.Text;

namespace LedgerScraper
{
    static class Util
    {
        public static string GetPartitionKey()
        {
            DateTime now = DateTime.UtcNow;
            if (now.Month == 4 && now.Year == 2020)
            {
                return "Season-202005";
            }
            return $"Season-{now:yyyyMM}";
        }

        public static string GetFactionRowKey()
        {
            return $"faction-{DateTime.UtcNow:yyyyMMdd-HH}";
        }

        public static string GetUserRowKey(string gamerTag)
        {
            return $"user-{DateTime.UtcNow:yyyyMMdd-HH}-{gamerTag}";
        }
    }
}
