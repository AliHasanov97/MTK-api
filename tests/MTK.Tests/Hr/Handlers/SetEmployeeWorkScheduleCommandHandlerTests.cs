using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class SetEmployeeWorkScheduleCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenNoCurrentSchedule_CreatesScheduleStartingToday()
    {
        var employeeId = Guid.NewGuid();
        var repository = new Mock<IEmployeeWorkScheduleRepository>();
        repository.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeWorkSchedule?)null);
        EmployeeWorkSchedule? added = null;
        repository.Setup(x => x.AddAsync(It.IsAny<EmployeeWorkSchedule>(), It.IsAny<CancellationToken>()))
            .Callback<EmployeeWorkSchedule, CancellationToken>((schedule, _) => added = schedule)
            .Returns(Task.CompletedTask);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var response = new SetEmployeeWorkScheduleResponse();
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<SetEmployeeWorkScheduleResponse>(It.IsAny<EmployeeWorkSchedule>())).Returns(response);
        var handler = new SetEmployeeWorkScheduleCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new SetEmployeeWorkScheduleCommand
        {
            EmployeeId = employeeId,
            Monday = 8m,
            Friday = 6m
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(added);
        Assert.Equal(employeeId, added!.EmployeeId);
        Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), added.EffectiveFrom);
        Assert.Equal(8m, added.Monday);
        Assert.Equal(6m, added.Friday);
        repository.Verify(x => x.AddAsync(It.IsAny<EmployeeWorkSchedule>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCurrentScheduleIsEmpty_UpdatesItInPlace()
    {
        var employeeId = Guid.NewGuid();
        var schedule = EmployeeWorkSchedule.Create(employeeId, new DateOnly(2026, 1, 1));
        var repository = new Mock<IEmployeeWorkScheduleRepository>();
        repository.Setup(x => x.GetByEmployeeIdAsync(employeeId, It.IsAny<CancellationToken>())).ReturnsAsync(schedule);
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<SetEmployeeWorkScheduleResponse>(schedule)).Returns(new SetEmployeeWorkScheduleResponse());
        var handler = new SetEmployeeWorkScheduleCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new SetEmployeeWorkScheduleCommand { EmployeeId = employeeId, Monday = 7m }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new DateOnly(2026, 1, 1), schedule.EffectiveFrom);
        Assert.Equal(7m, schedule.Monday);
        repository.Verify(x => x.AddAsync(It.IsAny<EmployeeWorkSchedule>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
