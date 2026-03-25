using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.API.Controllers;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.Controllers
{
    public class CategoryControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOk_WithCategories()
        {
            var svc = new Mock<ICategoryService>();
            var list = new List<CategoryResponse> { new CategoryResponse(Guid.NewGuid(), "Food", "Expense") };
            svc.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            var controller = new CategoriesController(svc.Object);
            var result = await controller.GetAll();

            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            ok!.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task Create_ReturnsOk_WithCreated()
        {
            var svc = new Mock<ICategoryService>();
            var response = new CategoryResponse(Guid.NewGuid(), "Travel", "Expense");
            svc.Setup(s => s.CreateAsync(It.IsAny<CreateCategoryRequest>())).ReturnsAsync(response);

            var controller = new CategoriesController(svc.Object);
            var req = new CreateCategoryRequest("Travel", "Expense");
            var result = await controller.Create(req);

            result.Result.Should().BeOfType<OkObjectResult>();
            var ok = result.Result as OkObjectResult;
            ok!.Value.Should().BeSameAs(response);
        }
    }
}
