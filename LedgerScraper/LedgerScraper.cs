using System;
using System.Collections.Generic;
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
        public static async void Run([TimerTrigger("%TimerTriggerSchedule%")]TimerInfo t, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"LedgerScraper Timer trigger function executed at: {DateTime.Now}.");

            var settings = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

            var cookies = JsonConvert.DeserializeObject<string[]>(settings["AuthCookies"]);
            var connectionString = settings["TableConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            var tableEntry = new DynamicTableEntity
            {
                PartitionKey = GetPartitionKey(),
                RowKey = GetRowKey()
            };

            try
            {
                foreach (var (url, abbr) in Factions)
                {
                    List<FactionData> factionData = new List<FactionData>();
                    foreach (var cookie in cookies)
                    {
                        var request = HttpWebRequest.Create($"https://www.seaofthieves.com/api/ledger/{url}");
                        request.Headers.Add($"Cookie: rat={cookie}");
                        var response = await request.GetResponseAsync();

                        var payload = await new StreamReader(response.GetResponseStream()).ReadToEndAsync().ConfigureAwait(false);
                        var data = JsonConvert.DeserializeObject<FactionData>(payload);
                        if (data.error)
                        {
                            // TODO: re-authenticate to get a new rat=<jwt token> value
                            throw new Exception("Invalid Auth cookie.");
                        }
                        factionData.Add(data);
                    }


                    foreach (var band in factionData[0].Bands)
                    {
                        var baseKey = $"{abbr}_{band.Index}_";
                        var results = band.Results.OrderBy(x => x.Score).ToArray();

                        tableEntry[$"{baseKey}hi_player"] = new EntityProperty(results[^1].GamerTag);
                        tableEntry[$"{baseKey}hi_rank"] = new EntityProperty(results[^1].Rank);
                        tableEntry[$"{baseKey}hi_score"] = new EntityProperty(results[^1].Score);
                    
                        tableEntry[$"{baseKey}lo_player"] = new EntityProperty(results[0].GamerTag);
                        tableEntry[$"{baseKey}lo_rank"] = new EntityProperty(results[0].Rank);
                        tableEntry[$"{baseKey}lo_score"] = new EntityProperty(results[0].Score);
                    }

                    foreach (var faction in factionData)
                    {
                        foreach (var band in faction.Bands)
                        {
                            if (band.Results.Count() == 3)
                            {
                                var results = band.Results.OrderBy(x => x.Score).ToArray();
                                tableEntry[$"{abbr}_{results[1].GamerTag}_rank"] = new EntityProperty(results[1].Rank);
                                tableEntry[$"{abbr}_{results[1].GamerTag}_score"] = new EntityProperty(results[1].Score);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to retrieve faction ledger information");
                return;
            }
            log.LogInformation("Writing scraped faction data to the Storage Table...");
            try
            {
                var table = tableClient.GetTableReference(settings["FactionTableName"]);
                var op = TableOperation.InsertOrReplace(tableEntry);
                var result = await table.ExecuteAsync(op).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to write entry to Storage Table.");
                return;
            }
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
