using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EmployeeWorkHistories.AddEmployeeWorkHistory;
using MTK.Modules.Hr.Application.EmployeeWorkHistories;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class AddEmployeeWorkHistoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEndDateIsBeforeStartDate_ReturnsValidationFailure()
    {
        var repository = new Mock<IEmployeeWorkHistoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new AddEmployeeWorkHistoryCommandHandler(
            repository.Object, unitOfWork.Object, Mock.Of<IMapper>(), Mock.Of<IUserContext>());
        var start = new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero);

        var result = await handler.Handle(new AddEmployeeWorkHistoryCommand
        {
            EmployeeId = Guid.NewGuid(),
            CompanyName = "Example Ltd",
            Position = "Accountant",
            StartDate = start,
            EndDate = start.AddDays(-1)
        }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeWorkHistoryErrors.InvalidDateRange, result.Error);
        repository.Verify(x => x.AddAsync(It.IsAny<EmployeeWorkHistory>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValid_PersistsAndMapsHistory()
    {
        var repository = new Mock<IEmployeeWorkHistoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var response = new AddEmployeeWorkHistoryResponse();
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<AddEmployeeWorkHistoryResponse>(It.IsAny<EmployeeWorkHistory>())).Returns(response);
        var handler = new AddEmployeeWorkHistoryCommandHandler(
            repository.Object, unitOfWork.Object, mapper.Object, Mock.Of<IUserContext>());

        var result = await handler.Handle(new AddEmployeeWorkHistoryCommand
        {
            EmployeeId = Guid.NewGuid(),
            CompanyName = "Example Ltd",
            Position = "Accountant",
            StartDate = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero)
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(response, result.Value);
        repository.Verify(x => x.AddAsync(It.Is<EmployeeWorkHistory>(h => h.CompanyName == "Example Ltd" && h.Position == "Accountant"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
