using System;

namespace LedgerScraper
{
    static class Util
    {
        public static string GetPartitionKey()
        {
            return GetPartitionKey(DateTime.UtcNow);
        }

        public static string GetPartitionKey(DateTime season)
        {
            if (season.Month == 4 && season.Year == 2020)
                return "Season-202005";
            return $"Season-{season:yyyyMM}";
        }

        public static string GetFactionRowKey()
        {
            return GetFactionRowKey(DateTime.UtcNow);
        }

        public static string GetFactionRowKey(DateTime season)
        {
            return $"faction-{season:yyyyMMdd-HH}";
        }

        public static string GetUserRowKey(string gamerTag)
        {
            return GetUserRowKey(gamerTag, DateTime.UtcNow);
        }

        public static string GetUserRowKey(string gamerTag, DateTime season)
        {
            return $"user-{season:yyyyMMdd-HH}-{gamerTag}";
        }
    }
}
