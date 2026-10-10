using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.GetVacationReturnApplicationById;

internal sealed class GetVacationReturnApplicationByIdQueryHandler
    : IQueryHandler<GetVacationReturnApplicationByIdQuery, GetVacationReturnApplicationByIdResponse>
{
    private readonly IVacationReturnApplicationRepository _repository;
    private readonly IMapper _mapper;

    public GetVacationReturnApplicationByIdQueryHandler(
        IVacationReturnApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetVacationReturnApplicationByIdResponse>> Handle(
        GetVacationReturnApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdWithDetailsAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetVacationReturnApplicationByIdResponse>(
                VacationReturnApplicationErrors.NotFound);

        var response = _mapper.Map<GetVacationReturnApplicationByIdResponse>(application);
        return Result.Success(response);
    }
}