using Moq;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Application.VacationApplications.CreateVacationApplication;
using MTK.Modules.Hr.Application.VacationApplications.Services;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationBalances;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class CreateVacationApplicationCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmployeeMissing_ReturnsFailureImmediately()
    {
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        var allocation = new Mock<IVacationApplicationAllocationService>();
        var overlap = new Mock<ILeaveOverlapService>();
        var balanceService = new Mock<IVacationBalanceService>();
        var balanceRepository = new Mock<IVacationBalanceRepository>();
        var applicationRepository = new Mock<IVacationApplicationRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var userContext = new Mock<IUserContext>();
        var handler = new CreateVacationApplicationCommandHandler(
            applicationRepository.Object,
            allocation.Object,
            employeeRepository.Object,
            unitOfWork.Object,
            overlap.Object,
            balanceService.Object,
            balanceRepository.Object,
            userContext.Object);

        var result = await handler.Handle(CreateCommand(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        allocation.VerifyNoOtherCalls();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesApplicationAndSaves()
    {
        var employee = Employee.Create(
            1, "Leyla", "Aliyeva", "Rauf", Gender.Female,
            new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), Guid.NewGuid(), Guid.NewGuid());
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        var allocation = new Mock<IVacationApplicationAllocationService>();
        var endDate = new DateTimeOffset(2027, 3, 5, 0, 0, 0, TimeSpan.Zero);
        allocation.Setup(x => x.CalculateDaysAndEndDateAsync(
                employee.Id, It.IsAny<DateTimeOffset>(), null, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success((3, endDate)));
        var overlap = new Mock<ILeaveOverlapService>();
        overlap.Setup(x => x.EnsureNoOverlapAsync(
                employee.Id, It.IsAny<DateTimeOffset>(), endDate, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());
        var balance = VacationBalance.Create(
            employee.Id, new DateOnly(2027, 1, 1), 15, new AdditionalVacationDaysBreakdown());
        var balanceService = new Mock<IVacationBalanceService>();
        balanceService.Setup(x => x.EnsureCurrentAsync(
                employee.Id, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(balance));
        var balanceRepository = new Mock<IVacationBalanceRepository>();
        balanceRepository.Setup(x => x.GetAllByEmployeeIdAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync([balance]);
        var applicationRepository = new Mock<IVacationApplicationRepository>();
        VacationApplication? addedApplication = null;
        applicationRepository.Setup(x => x.Add(It.IsAny<VacationApplication>()))
            .Callback<VacationApplication>(application => addedApplication = application);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var userContext = new Mock<IUserContext>();
        userContext.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        var handler = new CreateVacationApplicationCommandHandler(
            applicationRepository.Object,
            allocation.Object,
            employeeRepository.Object,
            unitOfWork.Object,
            overlap.Object,
            balanceService.Object,
            balanceRepository.Object,
            userContext.Object);

        var result = await handler.Handle(CreateCommand(employee.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(addedApplication);
        Assert.Equal(employee.Id, addedApplication!.EmployeeId);
        Assert.Equal(balance.Id, addedApplication.VacationBalanceId);
        Assert.Equal(3, addedApplication.TotalRequestedDays);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateVacationApplicationCommand CreateCommand(Guid? employeeId = null) => new(
        employeeId ?? Guid.NewGuid(),
        Guid.Empty,
        new DateTimeOffset(2027, 3, 1, 0, 0, 0, TimeSpan.Zero),
        null,
        3,
        "Annual leave");
}
