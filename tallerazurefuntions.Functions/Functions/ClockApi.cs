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
        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
                 [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "registration/{id}")] HttpRequest req,
                 [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
                 string id,
                 ILogger log)
        {
            log.LogInformation($"Update for todo: {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Registration registration = JsonConvert.DeserializeObject<Registration>(requestBody);

            // Validate todo id
            TableOperation findOperation = TableOperation.Retrieve<RegistrationEntity>("TODO", id);
            TableResult findResult = await registrationTable.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            // Update todo
            RegistrationEntity registrationEntity = (RegistrationEntity)findResult.Result;
            registrationEntity.Consolidated = registration.Consolidated;
            if (string.IsNullOrEmpty(registration?.IdEmployee.ToString()))
            {
                registrationEntity.IdEmployee = registration.IdEmployee;
            }

            TableOperation addOperation = TableOperation.Replace(registrationEntity);
            await registrationTable.ExecuteAsync(addOperation);

            string message = $"Todo: {id}, updated in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = registrationEntity
            });
        }

        [FunctionName(nameof(GetAllRegistration))]
        public static async Task<IActionResult> GetAllRegistration(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registration")] HttpRequest req,
                 [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
          ILogger log)
        {
            log.LogInformation("Get all registrations received.");

            TableQuery<RegistrationEntity> query = new TableQuery<RegistrationEntity>();
            TableQuerySegment<RegistrationEntity> registrations = await registrationTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all todos.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = registrations
            });
        }
        [FunctionName(nameof(GetTodoById))]
        public static IActionResult GetTodoById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "registration/{id}")] HttpRequest req,
           [Table("registration", "TODO", "{id}", Connection = "AzureWebJobsStorage")] RegistrationEntity registrationEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get registration by id: {id}, received.");

            if (registrationEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            string message = $"Registration: {registrationEntity.RowKey}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = registrationEntity
            });
        }
        [FunctionName(nameof(DeleteRegistration))]
        public static async Task<IActionResult> DeleteRegistration(
          [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "registration/{id}")] HttpRequest req,
          [Table("registration", "TODO", "{id}", Connection = "AzureWebJobsStorage")] RegistrationEntity registrationEntity,
          [Table("registration", Connection = "AzureWebJobsStorage")] CloudTable registrationTable,
          string id,
          ILogger log)
        {
            log.LogInformation($"Delete registration: {id}, received.");

            if (registrationEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Registration not found."
                });
            }

            await registrationTable.ExecuteAsync(TableOperation.Delete(registrationEntity));
            string message = $"Registration: {registrationEntity.RowKey}, deleted.";
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
