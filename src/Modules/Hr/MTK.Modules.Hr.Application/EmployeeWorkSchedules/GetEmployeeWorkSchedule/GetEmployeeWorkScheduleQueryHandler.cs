using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmployeeWorkSchedules;

namespace MTK.Modules.Hr.Application.EmployeeWorkSchedules.GetEmployeeWorkSchedule;

internal sealed class GetEmployeeWorkScheduleQueryHandler
    : IQueryHandler<GetEmployeeWorkScheduleQuery, GetEmployeeWorkScheduleResponse>
{
    private readonly IEmployeeWorkScheduleRepository _scheduleRepository;
    private readonly IMapper _mapper;

    public GetEmployeeWorkScheduleQueryHandler(
        IEmployeeWorkScheduleRepository scheduleRepository,
        IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmployeeWorkScheduleResponse>> Handle(
        GetEmployeeWorkScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByEmployeeIdAsync(request.EmployeeId, cancellationToken);
        if (schedule is null)
            return Result.Failure<GetEmployeeWorkScheduleResponse>(EmployeeWorkScheduleErrors.NotFound);

        return Result.Success(_mapper.Map<GetEmployeeWorkScheduleResponse>(schedule));
    }
}
