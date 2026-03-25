using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Domain.Common;
using PersonalFinanceTracker.Infrastructure.Services;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.Services
{
    public class BudgetServiceTests
    {
        [Fact]
        public async Task CreateBudgetAsync_ThrowsBadRequest_WhenEndBeforeStart()
        {
            var uow = new Mock<IUnitOfWork>();
            var svc = new BudgetService(uow.Object);

            var req = new CreateBudgetRequest(100m, DateTime.UtcNow.AddDays(1), DateTime.UtcNow, Guid.NewGuid());
            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.BadRequestException>(() => svc.CreateBudgetAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateBudgetAsync_ThrowsConflict_WhenOverlapping()
        {
            var uow = new Mock<IUnitOfWork>();
            uow.Setup(x => x.Budgets.IsOverlappingAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);
            var svc = new BudgetService(uow.Object);

            var req = new CreateBudgetRequest(100m, DateTime.UtcNow, DateTime.UtcNow.AddDays(10), Guid.NewGuid());
            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.ConflictException>(() => svc.CreateBudgetAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateBudgetAsync_ThrowsBadRequest_WhenCategoryIsIncome()
        {
            var uow = new Mock<IUnitOfWork>();
            var category = new Category { Id = Guid.NewGuid(), Type = CategoryTypes.Income };
            // Ensure Budgets repository is available to avoid NullReferenceException
            uow.SetupGet(x => x.Budgets).Returns(Mock.Of<IBudgetRepository>(r => r.IsOverlappingAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()) == Task.FromResult(false)));
            uow.SetupGet(x => x.Categories).Returns(Mock.Of<ICategoryRepository>(r => r.GetByIdAsync(It.IsAny<Guid>()) == Task.FromResult<Category?>(category)));

            var svc = new BudgetService(uow.Object);

            var req = new CreateBudgetRequest(100m, DateTime.UtcNow, DateTime.UtcNow.AddDays(10), Guid.NewGuid());

            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.BadRequestException>(() => svc.CreateBudgetAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateBudgetAsync_Succeeds_ReturnsResponse()
        {
            var uow = new Mock<IUnitOfWork>();
            var categoryId = Guid.NewGuid();
            var category = new Category { Id = categoryId, Name = "Food", Type = CategoryTypes.Expense };

            var budget = new Budget { Id = Guid.NewGuid(), CategoryId = categoryId, UserId = Guid.NewGuid(), LimitAmount = 1000m, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) };

            var budgetsRepo = new Mock<IBudgetRepository>();
            budgetsRepo.Setup(r => r.IsOverlappingAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(false);
            budgetsRepo.Setup(r => r.AddAsync(It.IsAny<Budget>())).Returns(Task.CompletedTask);

            var categoriesRepo = new Mock<ICategoryRepository>();
            categoriesRepo.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

            var transactionsRepo = new Mock<ITransactionRepository>();
            transactionsRepo.Setup(r => r.GetTotalSpentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(0m);

            var uowMock = new Mock<IUnitOfWork>();
            uowMock.SetupGet(x => x.Budgets).Returns(budgetsRepo.Object);
            uowMock.SetupGet(x => x.Categories).Returns(categoriesRepo.Object);
            uowMock.SetupGet(x => x.Transactions).Returns(transactionsRepo.Object);
            uowMock.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            var svc = new BudgetService(uowMock.Object); 

            var req = new CreateBudgetRequest(1000m, DateTime.UtcNow, DateTime.UtcNow.AddDays(30), categoryId);
            var result = await svc.CreateBudgetAsync(Guid.NewGuid(), req);

            result.Should().NotBeNull();
            result.CategoryName.Should().Be(category.Name);
            uowMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}
