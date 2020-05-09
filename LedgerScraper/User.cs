using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos.Table;
using LedgerScraper.Model;
using System.Globalization;

namespace LedgerScraper
{
    public static class User
    {
        [FunctionName("User")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/{gamerTag}")] HttpRequest req,
            string gamerTag,
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
                return new BadRequestObjectResult("Failed to initialize TableClient");
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

            var responseObject = new EntitiesResponse<UserEntity>();
            var table = tableClient.GetTableReference(settings["TableName"]);
            var query = table.CreateQuery<UserEntity>()
                .Where(x => x.PartitionKey == $"{Util.GetPartitionKey(season)}")
                .Where(x => x.RowKey.CompareTo($"user-") > 0)
                .Where(x => x.RowKey.CompareTo($"user-{season.AddMonths(1):yyyyMM}01-00") < 0);

            foreach (var entity in query)
            {
                if (entity.RowKey.EndsWith($"-{gamerTag}"))
                {
                    responseObject.Entries[entity.RowKey] = entity;
                }
            }
            var resp = new OkObjectResult(responseObject);
            resp.ContentTypes.Add(new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/json"));

            return resp;
        }
    }
}
