using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Employees.DeleteEmployee;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class DeleteEmployeeCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmployeeIsMissing_ReturnsNotFound()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new DeleteEmployeeCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeleteEmployeeCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmployeeWasHiredFromOrder_RejectsDeletion()
    {
        var employee = Employee.Create(
            1, "Ali", "Mammadov", "Rashad", Gender.Male,
            DateTimeOffset.UtcNow, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new DeleteEmployeeCommandHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(new DeleteEmployeeCommand(employee.Id), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.CannotDeleteHiredViaAcceptanceOrder, result.Error);
        repository.Verify(x => x.DeleteAsync(employee, null, It.IsAny<CancellationToken>()), Times.Never);
    }
}
