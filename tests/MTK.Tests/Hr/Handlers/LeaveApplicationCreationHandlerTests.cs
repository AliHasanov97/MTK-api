using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Application.UnpaidLeaveApplications.CreateUnpaidLeaveApplication;
using MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;
using MTK.Modules.Hr.Application.VacationReturnApplications.CreateVacationReturnApplication;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.VacationBalances;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class LeaveApplicationCreationHandlerTests
{
    [Fact]
    public async Task CreateUnpaidLeave_WhenEmployeeIsMissing_ReturnsNotFound()
    {
        var employeeRepository = MissingEmployeeRepository();
        var repository = new Mock<IUnpaidLeaveApplicationRepository>();
        var overlap = new Mock<ILeaveOverlapService>();
        var handler = new CreateUnpaidLeaveApplicationCommandHandler(
            repository.Object,
            employeeRepository.Object,
            Mock.Of<IUnitOfWork>(),
            overlap.Object,
            Mock.Of<IMapper>(),
            Mock.Of<IUserContext>());

        var result = await handler.Handle(new CreateUnpaidLeaveApplicationCommand { EmployeeId = Guid.NewGuid() }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        overlap.VerifyNoOtherCalls();
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateVacationReturn_WhenReturnDateIsInvalid_PropagatesOverlapError()
    {
        var employee = Employee.Create(
            1, "Leyla", "Aliyeva", "Rauf", MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            DateTimeOffset.UtcNow, Guid.NewGuid(), Guid.NewGuid());
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var overlap = new Mock<ILeaveOverlapService>();
        var overlapError = new MTK.Common.Domain.Abstractions.Error("Leave.InvalidReturnDate", "Invalid return date");
        overlap.Setup(x => x.EnsureReturnDateIsValidAsync(employee.Id, It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MTK.Common.Domain.Abstractions.Result.Failure(overlapError));
        var repository = new Mock<IVacationReturnApplicationRepository>();
        var handler = new CreateVacationReturnApplicationCommandHandler(
            repository.Object,
            employeeRepository.Object,
            Mock.Of<IUnitOfWork>(),
            overlap.Object,
            Mock.Of<IMapper>(),
            Mock.Of<IUserContext>());

        var result = await handler.Handle(
            new CreateVacationReturnApplicationCommand(employee.Id, DateTimeOffset.UtcNow, null),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(overlapError, result.Error);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateVacationCompensation_WhenNoBalanceExists_ReturnsFailureWithoutSaving()
    {
        var employee = Employee.Create(
            1, "Leyla", "Aliyeva", "Rauf", MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            DateTimeOffset.UtcNow, Guid.NewGuid(), Guid.NewGuid());
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var balanceRepository = new Mock<IVacationBalanceRepository>();
        balanceRepository.Setup(x => x.GetOldestWithRemainingDaysAsync(employee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((VacationBalance?)null);
        var repository = new Mock<IVacationCompensationApplicationRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateVacationCompensationApplicationCommandHandler(
            repository.Object,
            employeeRepository.Object,
            balanceRepository.Object,
            unitOfWork.Object,
            Mock.Of<IMapper>(),
            Mock.Of<IUserContext>());

        var result = await handler.Handle(new CreateVacationCompensationApplicationCommand
        {
            EmployeeId = employee.Id,
            RequestedDays = 4
        }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("VacationBalance.NoRemainingDays", result.Error.Code);
        repository.VerifyNoOtherCalls();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IEmployeeRepository> MissingEmployeeRepository()
    {
        var repository = new Mock<IEmployeeRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        return repository;
    }
}
