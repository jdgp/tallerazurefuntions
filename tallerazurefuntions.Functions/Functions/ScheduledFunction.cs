using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using tallerazurefunctions.Functions.Entities;
using tallerazurefuntions.Functions.Entities;

namespace tallerazurefunctions.Functions.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
             [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("IdEmployee", QueryComparisons.Equal, true);
            TableQuery<RegistrationEntity> query = new TableQuery<RegistrationEntity>().Where(filter);
            TableQuerySegment<RegistrationEntity> registrations = await registrationTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach (RegistrationEntity registration in registrations)
            {
                TableOperation addOperation = TableOperation.Insert(registration);
                await registrationTable.ExecuteAsync(addOperation);
            }

      


            log.LogInformation($"Deleted: {deleted} items at: {DateTime.Now}");
        }
    }
}
