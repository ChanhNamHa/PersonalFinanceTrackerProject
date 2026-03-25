using System.Collections.Generic;
using System.Linq;
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
    public class CategoryServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsMappedResponses()
        {
            var categories = new List<Category>
            {
                new Category { Id = System.Guid.NewGuid(), Name = "Food", Type = "Expense" },
                new Category { Id = System.Guid.NewGuid(), Name = "Salary", Type = "Income" }
            };

            var catRepo = new Mock<ICategoryRepository>();
            catRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Categories).Returns(catRepo.Object);

            var svc = new CategoryService(uow.Object);

            var result = (await svc.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result.Select(r => r.Name).Should().Contain(new[] { "Food", "Salary" });
        }

        [Fact]
        public async Task CreateAsync_Succeeds_WhenNotExists()
        {
            var req = new CreateCategoryRequest("Travel", "Expense");

            var catRepo = new Mock<ICategoryRepository>();
            catRepo.Setup(r => r.GetByNameAsync(req.Name)).ReturnsAsync((Category?)null);
            catRepo.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Categories).Returns(catRepo.Object);
            uow.Setup(x => x.CompleteAsync()).ReturnsAsync(1);

            var svc = new CategoryService(uow.Object);
            var res = await svc.CreateAsync(req);

            res.Name.Should().Be(req.Name);
            res.Type.Should().Be(req.Type);
            catRepo.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == req.Name && c.Type == req.Type)), Times.Once);
            uow.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsConflict_WhenExists()
        {
            var req = new CreateCategoryRequest("Food", "Expense");
            var existing = new Category { Id = System.Guid.NewGuid(), Name = req.Name };

            var catRepo = new Mock<ICategoryRepository>();
            catRepo.Setup(r => r.GetByNameAsync(req.Name)).ReturnsAsync(existing);

            var uow = new Mock<IUnitOfWork>();
            uow.SetupGet(x => x.Categories).Returns(catRepo.Object);

            var svc = new CategoryService(uow.Object);

            await Assert.ThrowsAsync<PersonalFinanceTracker.Application.Exceptions.ConflictException>(() => svc.CreateAsync(req));
        }
    }
}
