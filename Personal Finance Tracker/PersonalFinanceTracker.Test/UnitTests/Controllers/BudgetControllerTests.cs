using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.API.Controllers;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.Controllers
{
    public class BudgetControllerTests
    {
        private ClaimsPrincipal UserWithId(Guid id)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Create_ReturnsOk_WhenServiceCreates()
        {
            var svc = new Mock<IBudgetService>();
            var userId = Guid.NewGuid();
            var resp = new BudgetResponse(Guid.NewGuid(), 1000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), "Food", Guid.NewGuid(), 0m);
            svc.Setup(s => s.CreateBudgetAsync(userId, It.IsAny<CreateBudgetRequest>())).ReturnsAsync(resp);

            var controller = new BudgetsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var req = new CreateBudgetRequest(1000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), resp.CategoryId);
            var result = await controller.Create(req);

            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            ok!.Value.Should().BeSameAs(resp);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithBudgets()
        {
            var svc = new Mock<IBudgetService>();
            var userId = Guid.NewGuid();
            var list = new List<BudgetResponse> { new BudgetResponse(Guid.NewGuid(), 500m, DateTime.UtcNow, DateTime.UtcNow.AddDays(10), "Food", Guid.NewGuid(), 0m) };
            svc.Setup(s => s.GetUserBudgetsAsync(userId)).ReturnsAsync(list);

            var controller = new BudgetsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.GetAll();
            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var svc = new Mock<IBudgetService>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            svc.Setup(s => s.DeleteBudgetAsync(userId, id)).ReturnsAsync(true);

            var controller = new BudgetsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.Delete(id);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenFails()
        {
            var svc = new Mock<IBudgetService>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            svc.Setup(s => s.DeleteBudgetAsync(userId, id)).ReturnsAsync(false);

            var controller = new BudgetsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.Delete(id);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
