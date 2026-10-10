using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.GetEducationLeaveApplicationById;

internal sealed class GetEducationLeaveApplicationByIdQueryHandler
    : IQueryHandler<GetEducationLeaveApplicationByIdQuery, GetEducationLeaveApplicationByIdResponse>
{
    private readonly IEducationLeaveApplicationRepository _repository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IMapper _mapper;

    public GetEducationLeaveApplicationByIdQueryHandler(
        IEducationLeaveApplicationRepository repository,
        ReturnToWorkDateService returnToWorkDateService,
        IMapper mapper)
    {
        _repository = repository;
        _returnToWorkDateService = returnToWorkDateService;
        _mapper = mapper;
    }

    public async Task<Result<GetEducationLeaveApplicationByIdResponse>> Handle(
        GetEducationLeaveApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetEducationLeaveApplicationByIdResponse>(
                EducationLeaveApplicationErrors.NotFound);

        var response = _mapper.Map<GetEducationLeaveApplicationByIdResponse>(application);

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
