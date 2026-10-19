using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.VacationBalances.Commands.RecalculateVacationBalance;
using MTK.Modules.Hr.Application.VacationBalances.Queries.GetVacationBalanceByEmployeeId;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationBalances;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class RecalculateVacationBalanceCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEmployeeDoesNotExist_ReturnsNotFound()
    {
        var employees = new Mock<IEmployeeRepository>();
        employees.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);
        var service = new Mock<IVacationBalanceService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new RecalculateVacationBalanceCommandHandler(employees.Object, service.Object, unitOfWork.Object);

        var result = await handler.Handle(new RecalculateVacationBalanceCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(EmployeeErrors.NotFound, result.Error);
        service.VerifyNoOtherCalls();
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenEmployeeAndBalanceExist_RecalculatesAndSaves()
    {
        var employee = Employee.Create(
            3, "Leyla", "Aliyeva", "Rauf", MTK.Modules.Hr.Domain.JobApplications.Gender.Female,
            DateTimeOffset.UtcNow.AddYears(-20), Guid.NewGuid(), Guid.NewGuid(),
            isKarabakhWorker: true);
        var employees = new Mock<IEmployeeRepository>();
        employees.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var balance = VacationBalance.Create(employee.Id, new DateOnly(2026, 1, 1), 21, new AdditionalVacationDaysBreakdown());
        var service = new Mock<IVacationBalanceService>();
        service.Setup(x => x.EnsureCurrentAsync(employee.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(MTK.Common.Domain.Abstractions.Result.Success(balance));
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new RecalculateVacationBalanceCommandHandler(employees.Object, service.Object, unitOfWork.Object);

        var result = await handler.Handle(new RecalculateVacationBalanceCommand(employee.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(6, balance.ExperienceBonusDays);
        Assert.Equal(5, balance.KarabakhWorkerDays);
        Assert.Equal(11, balance.AdditionalDays);
        Assert.Equal(32, result.Value.TotalEntitledDays);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
