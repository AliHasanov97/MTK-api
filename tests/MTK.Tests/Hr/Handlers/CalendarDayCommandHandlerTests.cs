using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.CalendarDays;
using MTK.Modules.Hr.Application.CalendarDays.DeleteCalendarDay;
using MTK.Modules.Hr.Application.CalendarDays.UpdateCalendarDay;
using MTK.Modules.Hr.Domain.CalendarDays;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class CalendarDayCommandHandlerTests
{
    [Fact]
    public async Task UpdateCalendarDay_WhenDateConflicts_ReturnsDuplicateError()
    {
        var calendarDay = CalendarDay.Create("Old", new DateOnly(2027, 1, 1), CalendarDayType.Holiday).Value;
        var repository = new Mock<ICalendarDayRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(calendarDay.Id, It.IsAny<CancellationToken>())).ReturnsAsync(calendarDay);
        repository.Setup(x => x.ExistsByDateAsync(
            new DateOnly(2027, 1, 2), null, calendarDay.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new UpdateCalendarDayCommandHandler(repository.Object, unitOfWork.Object, Mock.Of<IMapper>());

        var result = await handler.Handle(new UpdateCalendarDayCommand
        {
            Id = calendarDay.Id,
            Name = "Updated",
            Date = new DateOnly(2027, 1, 2),
            DayType = CalendarDayType.Holiday
        }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CalendarDayErrors.DuplicateDate, result.Error);
        Assert.Equal("Old", calendarDay.Name);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCalendarDay_WhenNotFound_ReturnsNotFound()
    {
        var repository = new Mock<ICalendarDayRepository>();
        repository.Setup(x => x.GetByIdDefaultAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CalendarDay?)null);
        var handler = new DeleteCalendarDayCommandHandler(repository.Object);

        var result = await handler.Handle(new DeleteCalendarDayCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CalendarDayErrors.NotFound, result.Error);
        repository.Verify(x => x.DeleteAsync(It.IsAny<CalendarDay>(), null, It.IsAny<CancellationToken>()), Times.Never);
    }
}
