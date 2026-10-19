using AutoMapper;
using Moq;
using MTK.Modules.Hr.Application.CalendarDays;
using MTK.Modules.Hr.Application.CalendarDays.GetCalendarDaysByYear;
using MTK.Modules.Hr.Domain.CalendarDays;
using Xunit;

namespace MTK.Tests.Hr.Handlers;

public sealed class GetCalendarDaysByYearQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsYearStatisticsAndFiltersScheduleSpecificDays()
    {
        var holiday = CalendarDay.Create(
            "Holiday", new DateOnly(2027, 1, 1), CalendarDayType.Holiday).Value;
        var sixDayOnly = CalendarDay.Create(
            "Six-day schedule only", new DateOnly(2027, 1, 2), CalendarDayType.NonWorkingDay,
            applicableWorkingDays: MTK.Modules.Hr.Domain.Employees.WorkingDays.SixDays).Value;
        var repository = new Mock<ICalendarDayRepository>();
        repository.Setup(x => x.GetByYearAsync(2027, It.IsAny<CancellationToken>()))
            .ReturnsAsync([holiday, sixDayOnly]);
        var mapper = new Mock<IMapper>();
        mapper.Setup(x => x.Map<CalendarDayDto>(It.IsAny<CalendarDay>()))
            .Returns<CalendarDay>(day => new CalendarDayDto { Id = day.Id, Name = day.Name, Date = day.Date });
        var handler = new GetCalendarDaysByYearQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(
            new GetCalendarDaysByYearQuery(2027, MTK.Modules.Hr.Domain.Employees.WorkingDays.FiveDays),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2027, result.Value.Year);
        Assert.Equal(12, result.Value.Months.Count);
        Assert.Equal(31, result.Value.Months[0].TotalDays);
        Assert.Single(result.Value.Months[0].Days);
        Assert.Equal("Holiday", result.Value.Months[0].NonWorkingDaysList.Single().Name);
        Assert.Equal(365, result.Value.YearTotal.TotalDays);
        Assert.Equal(105, result.Value.YearTotal.NonWorkingDays);
    }
}
