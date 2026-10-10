using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.GetApplicationForChangeOfPositionById;

internal sealed class GetApplicationForChangeOfPositionByIdQueryHandler
    : IQueryHandler<GetApplicationForChangeOfPositionByIdQuery, GetApplicationForChangeOfPositionByIdResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetApplicationForChangeOfPositionByIdQueryHandler(
        IApplicationForChangeOfPositionRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetApplicationForChangeOfPositionByIdResponse>> Handle(
        GetApplicationForChangeOfPositionByIdQuery request,
        CancellationToken cancellationToken)
    {
     
            var application = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
            if (application is null)
                return Result.Failure<GetApplicationForChangeOfPositionByIdResponse>(
                    ApplicationForChangeOfPositionErrors.NotFound);

            return Result.Success(_mapper.Map<GetApplicationForChangeOfPositionByIdResponse>(application));
        
    }
}
