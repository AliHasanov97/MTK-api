using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.Modules.Hr.Application.CalendarDays.BulkCreateCalendarDays;

internal sealed class BulkCreateCalendarDaysCommandHandler
    : ICommandHandler<BulkCreateCalendarDaysCommand, BulkCreateCalendarDaysResponse>
{
    private const int MaxItems = 400;

    private readonly ICalendarDayRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkCreateCalendarDaysCommandHandler(ICalendarDayRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<BulkCreateCalendarDaysResponse>> Handle(
        BulkCreateCalendarDaysCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Days.Count == 0)
            return Result.Failure<BulkCreateCalendarDaysResponse>(
                new Error("CalendarDay.EmptyBulk", "Əlavə ediləcək gün seçilməyib"));

        if (command.Days.Count > MaxItems)
            return Result.Failure<BulkCreateCalendarDaysResponse>(
                new Error("CalendarDay.BulkTooLarge", $"Bir dəfəyə ən çox {MaxItems} gün əlavə etmək olar"));

        var created = new List<CalendarDay>();
        var skipped = new List<DateOnly>();

        foreach (var item in command.Days)
        {
            // Eyni sorğu daxilində təkrar və bazada mövcud olan tarixlər
            bool duplicateInBatch = created.Any(c => c.Date == item.Date &&
                (c.ApplicableWorkingDays == null || item.ApplicableWorkingDays == null ||
                 c.ApplicableWorkingDays == item.ApplicableWorkingDays));

            if (duplicateInBatch ||
                await _repository.ExistsByDateAsync(item.Date, item.ApplicableWorkingDays, cancellationToken: cancellationToken))
            {
                skipped.Add(item.Date);
                continue;
            }

            var result = CalendarDay.Create(item.Name, item.Date, item.DayType, item.Reason, item.ApplicableWorkingDays);
            if (result.IsFailure)
                return Result.Failure<BulkCreateCalendarDaysResponse>(result.Error);

            created.Add(result.Value);
        }

        foreach (var day in created)
            _repository.Add(day);

        if (created.Count > 0)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new BulkCreateCalendarDaysResponse
        {
            CreatedCount = created.Count,
            SkippedDates = skipped
        });
    }
}
