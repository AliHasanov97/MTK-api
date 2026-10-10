using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.Modules.Hr.Application.CalendarDays.CreateCalendarDay;

internal sealed class CreateCalendarDayCommandHandler : ICommandHandler<CreateCalendarDayCommand, CalendarDayDto>
{
    private readonly ICalendarDayRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCalendarDayCommandHandler(
        ICalendarDayRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CalendarDayDto>> Handle(
        CreateCalendarDayCommand command,
        CancellationToken cancellationToken)
    {
        // Check if calendar day already exists on this date with conflicting ApplicableWorkingDays
        bool exists = await _repository.ExistsByDateAsync(
            command.Date,
            command.ApplicableWorkingDays,
            cancellationToken: cancellationToken);
        if (exists)
        {
            return Result.Failure<CalendarDayDto>(CalendarDayErrors.DuplicateDate);
        }

        // Create calendar day
        var calendarDayResult = CalendarDay.Create(
            command.Name,
            command.Date,
            command.DayType,
            command.Reason,
            command.ApplicableWorkingDays);

        if (calendarDayResult.IsFailure)
        {
            return Result.Failure<CalendarDayDto>(calendarDayResult.Error);
        }

        _repository.Add(calendarDayResult.Value);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CalendarDayDto>(calendarDayResult.Value);
    }
}
