using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Domain.VacationApplications;

namespace MTK.Modules.Hr.Application.VacationApplications.GetVacationApplicationById;

internal sealed class GetVacationApplicationByIdQueryHandler
    : IQueryHandler<GetVacationApplicationByIdQuery, GetVacationApplicationByIdResponse>
{
    private readonly IVacationApplicationRepository _repository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IMapper _mapper;

    public GetVacationApplicationByIdQueryHandler(
        IVacationApplicationRepository repository,
        ReturnToWorkDateService returnToWorkDateService,
        IMapper mapper)
    {
        _repository = repository;
        _returnToWorkDateService = returnToWorkDateService;
        _mapper = mapper;
    }

    public async Task<Result<GetVacationApplicationByIdResponse>> Handle(
        GetVacationApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetVacationApplicationByIdResponse>(
                VacationApplicationErrors.NotFound);

        var response = _mapper.Map<GetVacationApplicationByIdResponse>(application);

        // ReturnToWorkDate real-time hesabla (əgər EndDate varsa)
        if (application.EndDate.HasValue)
        {
            var endDateOnly = DateOnly.FromDateTime(application.EndDate.Value.UtcDateTime);
            var result = await _returnToWorkDateService.CalculateAsync(
                application.EmployeeId,
                endDateOnly,
                cancellationToken);

            if (result.IsSuccess)
                response.ReturnToWorkDate = result.Value;
        }

        return Result.Success(response);
    }
}
