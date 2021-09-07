using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using tallerazurefuntions.Common.Models;
using tallerazurefuntions.Functions.Functions;
using todojdgp.Tests.Helpers;
using Xunit;

namespace todojdgp.Tests.Tests
{
    public class ClickApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateRegistration_Return_200()
        {
            // Arrenge
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Registration todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);

            // Act
            IActionResult response = await ClockApi.CreateRegistration(request, mockTodos, logger);

            // Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

    }
}
