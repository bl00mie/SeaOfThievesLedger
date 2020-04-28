using System;
using System.IO;
using System.Linq;
using System.Net;
using LedgerScraper.Model;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SeaOfThieves
{
    public static class LedgerScraper
    {
        static readonly (string url, string abbr)[] Factions = new []
        {
            ("ReapersBones","rb"),
            ("GoldHoarders", "gh"),
            ("MerchantAlliance", "ma"),
            ("OrderOfSouls", "os"),
            ("AthenasFortune", "af")
        };

        [FunctionName("LedgerScraper")]
        public static async void Run([TimerTrigger("%TimerTriggerSchedule%")]TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var settings = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var cookie = $"Cookie: {settings["DefaultCookie"]}";
            Console.WriteLine(cookie);

            var connectionString = settings["TableConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var tables = tableClient.ListTables();

            var tableEntry = new DynamicTableEntity();
            tableEntry.PartitionKey = GetPartitionKey();
            tableEntry.RowKey = GetPartitionKey();

            foreach (var faction in Factions)
            {
                var request = HttpWebRequest.Create($"https://www.seaofthieves.com/api/ledger/{faction.url}");
                request.Headers.Add(cookie);
                var response = await request.GetResponseAsync();

                var payload = await new StreamReader(response.GetResponseStream()).ReadToEndAsync().ConfigureAwait(false);
                var factionEntry = JsonConvert.DeserializeObject<FactionEntry>(payload);
                if (factionEntry.error)
                {
                    // TODO: re-authenticate to get a new rat=<jwt token> value
                }

                foreach (var band in factionEntry.Bands)
                {
                    var baseKey = $"{faction.abbr}-{band.Index}-";
                    var results = band.Results.OrderBy(x => x.Score).ToArray();

                    tableEntry[$"{baseKey}top-player"] = new EntityProperty(results[^1].GamerTag);
                    tableEntry[$"{baseKey}top-rank"] = new EntityProperty(results[^1].Rank);
                    tableEntry[$"{baseKey}top-score"] = new EntityProperty(results[^1].Score);
                    
                    tableEntry[$"{baseKey}bot-player"] = new EntityProperty(results[0].GamerTag);
                    tableEntry[$"{baseKey}bot-rank"] = new EntityProperty(results[0].Rank);
                    tableEntry[$"{baseKey}bot-score"] = new EntityProperty(results[0].Score);

                }
            }
            var table = tableClient.GetTableReference(settings["TableName"]);
            Console.WriteLine(table.Exists());
            var op = TableOperation.InsertOrReplace(tableEntry);
            var result = await table.ExecuteAsync(op).ConfigureAwait(false);


        }

        public static string GetPartitionKey()
        {
            DateTime now = DateTime.UtcNow;
            if (now.Month == 4 && now.Year == 2020)
            {
                return "Season-202005";
            }
            return $"Season-{now:yyyyMM}";
        }

        public static string GetRowKey()
        {
            return $"{DateTime.UtcNow:yyyyMMdd-HH}";
        }
    }
}
