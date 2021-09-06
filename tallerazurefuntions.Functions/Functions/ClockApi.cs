using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using tallerazurefuntions.Common.Models;
using tallerazurefuntions.Common.Responses;
using tallerazurefuntions.Functions.Entities;

namespace tallerazurefuntions.Functions.Functions
{
    public static class ClockApi
    {
        [FunctionName("ClockApi")]
        public static async Task<IActionResult> CreateRegistration(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "registration")] HttpRequest req,
      [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
      ILogger log)
        {
            log.LogInformation("Reciaved a new todo.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registration = JsonConvert.DeserializeObject<Registration>(requestBody);

            if (string.IsNullOrEmpty(registration?.IdEmployee.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription"
                });
            }

            RegistrationEntity registrationEntity = new RegistrationEntity
            {
                Date = DateTime.UtcNow,
                ETag = "*",
                PartitionKey = "TODO",
                Consolidated = registration.Consolidated,
                Type = registration.Type,
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = registration.IdEmployee
            };

            TableOperation addOperation = TableOperation.Insert(registrationEntity);
            await registrationTable.ExecuteAsync(addOperation);

            string message = "New registration storad in table";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = registrationEntity
            });
        }
    }
}
