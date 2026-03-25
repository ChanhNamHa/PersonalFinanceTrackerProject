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
    public class TransactionControllerTests
    {
        private ClaimsPrincipal UserWithId(Guid id)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) }, "TestAuth");
            return new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenServiceReturnsDto()
        {
            var svc = new Mock<ITransactionService>();
            var userId = Guid.NewGuid();
            var dto = new TransactionDTO(userId, 100m, "note", DateTime.UtcNow, "Cat", Guid.NewGuid());

            svc.Setup(s => s.CreateTransactionAsync(userId, It.IsAny<CreateTransactionRequest>()))
               .ReturnsAsync(dto);

            var controller = new TransactionsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var req = new CreateTransactionRequest { Amount = 100m, CategoryId = dto.CategoryId, TransactionDate = DateTime.UtcNow, Note = "note" };
            var result = await controller.Create(req);

            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var created = result.Result as CreatedAtActionResult;
            created!.Value.Should().BeEquivalentTo(dto);
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WithList()
        {
            var svc = new Mock<ITransactionService>();
            var userId = Guid.NewGuid();
            var list = new List<TransactionDTO> { new TransactionDTO(Guid.NewGuid(), 50m, null, DateTime.UtcNow, "Cat", Guid.NewGuid()) };
            svc.Setup(s => s.GetUserTransactionsAsync(userId)).ReturnsAsync(list);

            var controller = new TransactionsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.GetAll();
            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenNull()
        {
            var svc = new Mock<ITransactionService>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            svc.Setup(s => s.GetTransactionByIdAsync(userId, id)).ReturnsAsync((TransactionDTO?)null);

            var controller = new TransactionsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.GetById(id);
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            var svc = new Mock<ITransactionService>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            svc.Setup(s => s.DeleteTransactionAsync(userId, id)).ReturnsAsync(true);

            var controller = new TransactionsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.Delete(id);
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenFails()
        {
            var svc = new Mock<ITransactionService>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            svc.Setup(s => s.DeleteTransactionAsync(userId, id)).ReturnsAsync(false);

            var controller = new TransactionsController(svc.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = UserWithId(userId) } }
            };

            var result = await controller.Delete(id);
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
