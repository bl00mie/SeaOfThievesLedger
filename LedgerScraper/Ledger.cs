using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;
using LedgerScraper.Model;
using System.Globalization;

namespace LedgerScraper
{
    public static class Ledger
    {
        [FunctionName("Ledger")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            IConfigurationRoot settings = null;
            string[] cookies = null;
            string connectionString = null;
            CloudStorageAccount storageAccount = null;
            CloudTableClient tableClient = null;

            try
            {
                settings = new ConfigurationBuilder()
                    .SetBasePath(context.FunctionAppDirectory)
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                cookies = JsonConvert.DeserializeObject<string[]>(settings["AuthCookies"]);
                connectionString = settings["AzureWebJobsStorage"];
                storageAccount = CloudStorageAccount.Parse(connectionString);
                tableClient = storageAccount.CreateCloudTableClient();
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to initialize Table Client.");
                return new BadRequestResult();
            }

            DateTime season = DateTime.UtcNow;
            
            var seasonStr = req.Query["season"].ToString();
            if (seasonStr != "")
            {
                try
                {
                    season = DateTime.ParseExact(req.Query["season"], "yyyyMM", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    return new BadRequestObjectResult("Invalid season parameter. It should be a string formatted like 'yyyyMM'");
                }
            }

            var responseObject = new EntitiesResponse<LedgerEntity>();
            var table = tableClient.GetTableReference(settings["TableName"]);
            var query = table.CreateQuery<LedgerEntity>()
                .Where(x => x.PartitionKey == $"{Util.GetPartitionKey(season)}")
                .Where(x => x.RowKey.CompareTo($"faction-") > 0)
                .Where(x => x.RowKey.CompareTo($"faction-{season.AddMonths(1):yyyyMM}01-00") < 0);

            foreach (var thing in query)
            {
                responseObject.Entries[thing.RowKey] = thing;
            }
            var resp = new OkObjectResult(responseObject);
            resp.ContentTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            return resp;
        }
    }
}
