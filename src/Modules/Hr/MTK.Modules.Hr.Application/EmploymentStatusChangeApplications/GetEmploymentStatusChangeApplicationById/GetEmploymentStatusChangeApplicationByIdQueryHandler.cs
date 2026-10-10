using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.GetEmploymentStatusChangeApplicationById;

internal sealed class GetEmploymentStatusChangeApplicationByIdQueryHandler
    : IQueryHandler<GetEmploymentStatusChangeApplicationByIdQuery, GetEmploymentStatusChangeApplicationByIdResponse>
{
    private readonly IEmploymentStatusChangeApplicationRepository _repository;
    private readonly IMapper _mapper;

    public GetEmploymentStatusChangeApplicationByIdQueryHandler(
        IEmploymentStatusChangeApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetEmploymentStatusChangeApplicationByIdResponse>> Handle(
        GetEmploymentStatusChangeApplicationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (application == null)
            return Result.Failure<GetEmploymentStatusChangeApplicationByIdResponse>(
                EmploymentStatusChangeApplicationErrors.NotFound);

        var response = _mapper.Map<GetEmploymentStatusChangeApplicationByIdResponse>(application);

        return Result.Success(response);
    }
}
