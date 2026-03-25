using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using PersonalFinanceTracker.Application.DTOs;
using PersonalFinanceTracker.Application.Interfaces;
using PersonalFinanceTracker.Domain.Entities;
using PersonalFinanceTracker.Infrastructure.Services;
using Xunit;

namespace PersonalFinanceTracker.Tests.UnitTests.Services
{
    public class TransactionServiceTests
    {
        [Fact]
        public async Task CreateTransactionAsync_ThrowsNotFound_WhenCategoryMissing()
        {
            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Categories).Returns(Mock.Of<ICategoryRepository>(r => r.GetByIdAsync(It.IsAny<Guid>()) == Task.FromResult<Category?>(null)));

            var svc = new TransactionService(uow.Object);

            var req = new CreateTransactionRequest { Amount = 10m, CategoryId = Guid.NewGuid(), TransactionDate = DateTime.UtcNow };

            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.NotFoundException>(() => svc.CreateTransactionAsync(Guid.NewGuid(), req));
        }

        [Fact]
        public async Task CreateTransactionAsync_ThrowsBudgetExceeded_WhenOverLimit()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category { Id = categoryId, Name = "Food" };
            uowMock.SetupGet(x => x.Categories).Returns(Mock.Of<ICategoryRepository>(r => r.GetByIdAsync(categoryId) == Task.FromResult<Category?>(category)));

            var budget = new Budget { Id = Guid.NewGuid(), UserId = userId, CategoryId = categoryId, LimitAmount = 100m, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(10) };
            uowMock.SetupGet(x => x.Budgets).Returns(Mock.Of<IBudgetRepository>(r => r.GetActiveBudgetAsync(userId, categoryId, It.IsAny<DateTime>()) == Task.FromResult<Budget?>(budget)));

            // Existing spent = 60, new amount = 50 -> total 110 > 100 => exceed
            uowMock.SetupGet(x => x.Transactions).Returns(Mock.Of<ITransactionRepository>(r => r.GetTotalSpentAsync(userId, categoryId, budget.StartDate, budget.EndDate) == Task.FromResult<decimal>(60m)));

            var svc = new TransactionService(uowMock.Object);

            var req = new CreateTransactionRequest { Amount = 50m, CategoryId = categoryId, TransactionDate = DateTime.UtcNow };

            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.BudgetExceededException>(() => svc.CreateTransactionAsync(userId, req));
        }

        [Fact]
        public async Task CreateTransactionAsync_Succeeds_ReturnsDto()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var categoryId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var category = new Category { Id = categoryId, Name = "Food" };
            var categoriesRepo = new Mock<ICategoryRepository>();
            categoriesRepo.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync(category);

            var transactionsRepo = new Mock<ITransactionRepository>();
            transactionsRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>())).Returns(Task.CompletedTask);
            transactionsRepo.Setup(r => r.GetTotalSpentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(0m);

            uowMock.SetupGet(x => x.Categories).Returns(categoriesRepo.Object);
            uowMock.SetupGet(x => x.Transactions).Returns(transactionsRepo.Object);
            uowMock.SetupGet(x => x.Budgets).Returns(Mock.Of<IBudgetRepository>(r => r.GetActiveBudgetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>()) == Task.FromResult<Budget?>(null)));
            uowMock.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            var svc = new TransactionService(uowMock.Object);

            var req = new CreateTransactionRequest { Amount = 25m, CategoryId = categoryId, TransactionDate = DateTime.UtcNow, Note = "ok" };
            var res = await svc.CreateTransactionAsync(userId, req);

            res.Should().NotBeNull();
            res.CategoryName.Should().Be(category.Name);
            transactionsRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
            uowMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteTransactionAsync_ReturnsTrue_WhenSuccess()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();
            var transaction = new Transaction { Id = id, UserId = userId };

            var transactionsRepo = new Mock<ITransactionRepository>();
            transactionsRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(transaction);
            transactionsRepo.Setup(r => r.Delete(It.IsAny<Transaction>()));

            uowMock.SetupGet(x => x.Transactions).Returns(transactionsRepo.Object);
            uowMock.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            var svc = new TransactionService(uowMock.Object);

            var result = await svc.DeleteTransactionAsync(userId, id);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTransactionAsync_ReturnsFalse_WhenNotFoundOrDifferentUser()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var userId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var transactionsRepo = new Mock<ITransactionRepository>();
            transactionsRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Transaction?)null);

            uowMock.SetupGet(x => x.Transactions).Returns(transactionsRepo.Object);

            var svc = new TransactionService(uowMock.Object);

            var result = await svc.DeleteTransactionAsync(userId, id);
            result.Should().BeFalse();
        }
    }
}
