using Moq;
using MTK.Common.Application.EventBus;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EmployeeWorkSchedules;
using MTK.Modules.Hr.Application.Employees.Events;
using MTK.Modules.Hr.Application.VacationBalances.Events;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using MTK.Modules.Hr.Domain.EmployeeWorkHistories;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Employees.Events;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.VacationBalances;
using MTK.Modules.Hr.IntegrationEvents.Employees;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class EmployeeDomainEventHandlerTests
{
    [Fact]
    public async Task EmployeeCreatedHandler_PublishesIntegrationEvent()
    {
        var eventBus = new Mock<IEventBus>();
        eventBus.Setup(x => x.PublishAsync(It.IsAny<EmployeeCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new EmployeeCreatedDomainEventHandler(eventBus.Object);
        var employeeId = Guid.NewGuid();

        await handler.Handle(new EmployeeCreatedDomainEvent
        {
            EmployeeId = employeeId,
            FullName = "Leyla Aliyeva",
            BaseSalary = 1200m,
            IsActive = true
        });

        eventBus.Verify(x => x.PublishAsync(
            It.Is<EmployeeCreatedIntegrationEvent>(e => e.EmployeeId == employeeId && e.FullName == "Leyla Aliyeva" && e.BaseSalary == 1200m),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EmployeeUpdatedHandler_PublishesIntegrationEvent()
    {
        var eventBus = new Mock<IEventBus>();
        eventBus.Setup(x => x.PublishAsync(It.IsAny<EmployeeUpdatedIntegrationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new EmployeeUpdatedDomainEventHandler(eventBus.Object);
        var employeeId = Guid.NewGuid();

        await handler.Handle(new EmployeeUpdatedDomainEvent
        {
            EmployeeId = employeeId,
            FullName = "Leyla Aliyeva",
            BaseSalary = 1500m,
            IsActive = false
        });

        eventBus.Verify(x => x.PublishAsync(
            It.Is<EmployeeUpdatedIntegrationEvent>(e => e.EmployeeId == employeeId && e.BaseSalary == 1500m && !e.IsActive),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EmployeeCreatedWorkScheduleHandler_CreatesEmptyScheduleAtStartDate()
    {
        var repository = new Mock<IEmployeeWorkScheduleRepository>();
        EmployeeWorkSchedule? addedSchedule = null;
        repository.Setup(x => x.AddAsync(It.IsAny<EmployeeWorkSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<EmployeeWorkSchedule, CancellationToken>((schedule, _) => addedSchedule = schedule)
            .Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new EmployeeCreatedWorkScheduleHandler(repository.Object, unitOfWork.Object);
        var employeeId = Guid.NewGuid();
        var start = new DateTimeOffset(2024, 2, 3, 12, 0, 0, TimeSpan.FromHours(4));

        await handler.Handle(new EmployeeCreatedDomainEvent { EmployeeId = employeeId, StartWorkDate = start });

        Assert.NotNull(addedSchedule);
        Assert.Equal(employeeId, addedSchedule!.EmployeeId);
        Assert.Equal(new DateOnly(2024, 2, 3), addedSchedule.EffectiveFrom);
        Assert.Null(addedSchedule.Monday);
        Assert.Null(addedSchedule.Sunday);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WorkHistoryChangedHandler_WhenCurrentBalanceExists_RecalculatesAdditionalLeaveDays()
    {
        var employee = Employee.Create(
            8, "Leyla", "Aliyeva", "Rauf", Gender.Female,
            DateTimeOffset.UtcNow.AddYears(-20), Guid.NewGuid(), Guid.NewGuid(),
            childrenUnder14Count: 2, isKarabakhWorker: true);
        var employeeRepository = new Mock<IEmployeeRepository>();
        employeeRepository.Setup(x => x.GetByIdDefaultAsync(employee.Id, It.IsAny<CancellationToken>())).ReturnsAsync(employee);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var balance = VacationBalance.Create(employee.Id, today, 21, new AdditionalVacationDaysBreakdown());
        var balanceRepository = new Mock<IVacationBalanceRepository>();
        balanceRepository.Setup(x => x.GetCurrentAsync(employee.Id, today, It.IsAny<CancellationToken>())).ReturnsAsync(balance);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new EmployeeWorkHistoryChangedDomainEventHandler(
            employeeRepository.Object, balanceRepository.Object, unitOfWork.Object);

        await handler.Handle(new EmployeeWorkHistoryChangedDomainEvent { EmployeeId = employee.Id });

        Assert.Equal(6, balance.ExperienceBonusDays);
        Assert.Equal(5, balance.KarabakhWorkerDays);
        Assert.Equal(2, balance.ChildrenUnder14Days);
        Assert.Equal(13, balance.AdditionalDays);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
