using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules.SetEmployeeWorkSchedule;

internal sealed class SetEmployeeWorkScheduleCommandHandler
    : ICommandHandler<SetEmployeeWorkScheduleCommand, SetEmployeeWorkScheduleResponse>
{
    private readonly IEmployeeWorkScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SetEmployeeWorkScheduleCommandHandler(
        IEmployeeWorkScheduleRepository scheduleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SetEmployeeWorkScheduleResponse>> Handle(
        SetEmployeeWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Ən son (cari) schedule-u tap
        var currentSchedule = await _scheduleRepository.GetByEmployeeIdAsync(
            request.EmployeeId, cancellationToken);

        if (currentSchedule is not null)
        {
            // Schedule boşdursa (ilk dəfə doldurulur) - mövcud record-u UPDATE et
            // Bu halda EffectiveFrom = StartWorkDate olaraq qalır
            if (IsScheduleEmpty(currentSchedule))
            {
                currentSchedule.Update(
                    request.Monday,
                    request.Tuesday,
                    request.Wednesday,
                    request.Thursday,
                    request.Friday,
                    request.Saturday,
                    request.Sunday);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(_mapper.Map<SetEmployeeWorkScheduleResponse>(currentSchedule));
            }

            // Bugünkü tarixdə schedule varsa - UPDATE et
            var todaySchedule = await _scheduleRepository.GetByEmployeeAndDateAsync(
                request.EmployeeId, today, cancellationToken);

            if (todaySchedule is not null)
            {
                todaySchedule.Update(
                    request.Monday,
                    request.Tuesday,
                    request.Wednesday,
                    request.Thursday,
                    request.Friday,
                    request.Saturday,
                    request.Sunday);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success(_mapper.Map<SetEmployeeWorkScheduleResponse>(todaySchedule));
            }
        }

        // Fərqli gündə dəyişiklik - yeni record yarat bugünkü tarixlə
        var schedule = EmployeeWorkSchedule.Create(
            request.EmployeeId,
            today,
            request.Monday,
            request.Tuesday,
            request.Wednesday,
            request.Thursday,
            request.Friday,
            request.Saturday,
            request.Sunday);

        await _scheduleRepository.AddAsync(schedule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(_mapper.Map<SetEmployeeWorkScheduleResponse>(schedule));
    }

    /// <summary>
    /// Schedule-un bütün günləri boşdursa true qaytarır
    /// </summary>
    private static bool IsScheduleEmpty(EmployeeWorkSchedule schedule)
    {
        return schedule.Monday == null &&
               schedule.Tuesday == null &&
               schedule.Wednesday == null &&
               schedule.Thursday == null &&
               schedule.Friday == null &&
               schedule.Saturday == null &&
               schedule.Sunday == null;
    }
}
