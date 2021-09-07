using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using tallerazurefuntions.Common.Models;
using tallerazurefuntions.Functions.Entities;

namespace todojdgp.Tests.Helpers
{
    public class TestFactory
    {
        public static RegistrationEntity GetTodoEntity()
        {
            return new RegistrationEntity
            {
                ETag = "*",
                PartitionKey = "TODO",
                RowKey = Guid.NewGuid().ToString(),
                IdEmployee = 1315535,
                Consolidated = false,
                Type = 1,
                Date = DateTime.UtcNow

            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid todoId, Registration todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/{todoId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Guid todoId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{todoId}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(Registration todoRequest)
        {
            string request = JsonConvert.SerializeObject(todoRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request)
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Registration GetTodoRequest()
        {
            return new Registration
            {
                IdEmployee = 1315535,
                Consolidated = false,
                Type = 1,
                Date = DateTime.UtcNow
            };
        }

        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
