using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.CalendarDays.DeleteCalendarDay;

public sealed record DeleteCalendarDayCommand(Guid Id) : ICommand;
