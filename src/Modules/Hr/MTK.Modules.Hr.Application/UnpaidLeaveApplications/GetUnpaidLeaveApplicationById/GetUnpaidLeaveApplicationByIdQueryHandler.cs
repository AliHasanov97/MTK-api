using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.GetUnpaidLeaveApplicationById;

internal sealed class GetUnpaidLeaveApplicationByIdQueryHandler
    : IQueryHandler<GetUnpaidLeaveApplicationByIdQuery, GetUnpaidLeaveApplicationByIdResponse>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IMapper _mapper;

    public GetUnpaidLeaveApplicationByIdQueryHandler(
        IUnpaidLeaveApplicationRepository repository,
        ReturnToWorkDateService returnToWorkDateService,
        IMapper mapper)
    {
        _repository = repository;
        _returnToWorkDateService = returnToWorkDateService;
        _mapper = mapper;
    }

    public async Task<Result<GetUnpaidLeaveApplicationByIdResponse>> Handle(
        GetUnpaidLeaveApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetUnpaidLeaveApplicationByIdResponse>(
                UnpaidLeaveApplicationErrors.NotFound);

        var response = _mapper.Map<GetUnpaidLeaveApplicationByIdResponse>(application);

        // ReturnToWorkDate real-time hesabla
        var endDateOnly = DateOnly.FromDateTime(application.EndDate.UtcDateTime);
        var result = await _returnToWorkDateService.CalculateAsync(
            application.EmployeeId,
            endDateOnly,
            cancellationToken);

        if (result.IsSuccess)
            response.ReturnToWorkDate = result.Value;

        return Result.Success(response);
    }
}
