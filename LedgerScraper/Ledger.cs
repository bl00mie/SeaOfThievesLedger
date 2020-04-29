using System;
using System.Threading.Tasks;
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

namespace LedgerScraper
{
    public static class Ledger
    {
        [FunctionName("Ledger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ledger")] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
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

            var nextSeason = DateTime.UtcNow.AddMonths(1);

            var responseObject = new LedgerResponse();
            var table = tableClient.GetTableReference(settings["TableName"]);
            var query = table.CreateQuery<LedgerEntity>()
                .Where(x => x.PartitionKey == $"{Util.GetPartitionKey()}")
                .Where(x => x.RowKey.CompareTo($"faction-") > 0)
                .Where(x => x.RowKey.CompareTo($"faction-{nextSeason:yyyyMM}01-00") < 0);

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
