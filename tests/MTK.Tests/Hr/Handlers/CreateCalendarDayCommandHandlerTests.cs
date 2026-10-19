using AutoMapper;
using Moq;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.CalendarDays;
using MTK.Modules.Hr.Application.CalendarDays.CreateCalendarDay;
using MTK.Modules.Hr.Domain.CalendarDays;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class CreateCalendarDayCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenDateAlreadyExists_ReturnsFailureWithoutSaving()
    {
        var repository = new Mock<ICalendarDayRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        var date = new DateOnly(2027, 1, 1);
        repository.Setup(x => x.ExistsByDateAsync(date, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var handler = new CreateCalendarDayCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);

        var result = await handler.Handle(new CreateCalendarDayCommand
        {
            Name = "Holiday",
            Date = date,
            DayType = CalendarDayType.Holiday
        }, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(CalendarDayErrors.DuplicateDate, result.Error);
        repository.Verify(x => x.Add(It.IsAny<CalendarDay>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValid_CreatesCalendarDayAndSaves()
    {
        var repository = new Mock<ICalendarDayRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        CalendarDay? addedDay = null;
        var expectedDto = new CalendarDayDto { Name = "Holiday", Date = new DateOnly(2027, 1, 1) };

        repository.Setup(x => x.ExistsByDateAsync(
                It.IsAny<DateOnly>(), It.IsAny<MTK.Modules.Hr.Domain.Employees.WorkingDays?>(),
                null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        repository.Setup(x => x.Add(It.IsAny<CalendarDay>()))
            .Callback<CalendarDay>(day => addedDay = day);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(x => x.Map<CalendarDayDto>(It.IsAny<CalendarDay>())).Returns(expectedDto);

        var handler = new CreateCalendarDayCommandHandler(repository.Object, unitOfWork.Object, mapper.Object);
        var result = await handler.Handle(new CreateCalendarDayCommand
        {
            Name = " Holiday ",
            Date = new DateOnly(2027, 1, 1),
            DayType = CalendarDayType.Holiday
        }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(expectedDto, result.Value);
        Assert.NotNull(addedDay);
        Assert.Equal("Holiday", addedDay!.Name);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
