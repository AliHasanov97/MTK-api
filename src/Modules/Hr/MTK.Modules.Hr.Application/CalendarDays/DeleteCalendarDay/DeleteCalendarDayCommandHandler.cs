using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.Modules.Hr.Application.CalendarDays.DeleteCalendarDay;

internal sealed class DeleteCalendarDayCommandHandler : ICommandHandler<DeleteCalendarDayCommand>
{
    private readonly ICalendarDayRepository _repository;

    public DeleteCalendarDayCommandHandler(ICalendarDayRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteCalendarDayCommand command,
        CancellationToken cancellationToken)
    {
        var calendarDay = await _repository.GetByIdDefaultAsync(command.Id, cancellationToken);
        if (calendarDay is null)
        {
            return Result.Failure(CalendarDayErrors.NotFound);
        }

        await _repository.DeleteAsync(calendarDay, null, cancellationToken);

        return Result.Success();
    }
}
