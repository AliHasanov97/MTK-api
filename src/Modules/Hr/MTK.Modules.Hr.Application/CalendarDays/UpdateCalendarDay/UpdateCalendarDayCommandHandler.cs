using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.CalendarDays;

namespace MTK.Modules.Hr.Application.CalendarDays.UpdateCalendarDay;

internal sealed class UpdateCalendarDayCommandHandler : ICommandHandler<UpdateCalendarDayCommand, CalendarDayDto>
{
    private readonly ICalendarDayRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateCalendarDayCommandHandler(
        ICalendarDayRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CalendarDayDto>> Handle(
        UpdateCalendarDayCommand command,
        CancellationToken cancellationToken)
    {
        // Get calendar day
        var calendarDay = await _repository.GetByIdDefaultAsync(command.Id, cancellationToken);
        if (calendarDay is null)
        {
            return Result.Failure<CalendarDayDto>(CalendarDayErrors.NotFound);
        }

        // Check if date already exists with conflicting ApplicableWorkingDays (excluding current calendar day)
        bool exists = await _repository.ExistsByDateAsync(
            command.Date,
            command.ApplicableWorkingDays,
            command.Id,
            cancellationToken);
        if (exists)
        {
            return Result.Failure<CalendarDayDto>(CalendarDayErrors.DuplicateDate);
        }

        // Update calendar day
        var updateResult = calendarDay.Update(
            command.Name,
            command.Date,
            command.DayType,
            command.Reason,
            command.ApplicableWorkingDays);

        if (updateResult.IsFailure)
        {
            return Result.Failure<CalendarDayDto>(updateResult.Error);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CalendarDayDto>(calendarDay);
    }
}
