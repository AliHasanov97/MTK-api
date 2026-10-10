using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.GetVacationCompensationApplicationById;

internal sealed class GetVacationCompensationApplicationByIdQueryHandler
    : IQueryHandler<GetVacationCompensationApplicationByIdQuery, GetVacationCompensationApplicationByIdResponse>
{
    private readonly IVacationCompensationApplicationRepository _repository;
    private readonly IMapper _mapper;

    public GetVacationCompensationApplicationByIdQueryHandler(
        IVacationCompensationApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetVacationCompensationApplicationByIdResponse>> Handle(
        GetVacationCompensationApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithLinesAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetVacationCompensationApplicationByIdResponse>(
                VacationCompensationErrors.NotFound(request.Id));

        return Result.Success(_mapper.Map<GetVacationCompensationApplicationByIdResponse>(application));
    }
}