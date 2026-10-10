using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.GetWarningById;

internal sealed class GetWarningByIdQueryHandler : IQueryHandler<GetWarningByIdQuery, GetWarningByIdResponse>
{
    private readonly IWarningRepository _repository;
    private readonly IMapper _mapper;

    public GetWarningByIdQueryHandler(
        IWarningRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetWarningByIdResponse>> Handle(GetWarningByIdQuery request, CancellationToken cancellationToken)
    {
        var warning = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (warning is null)
            return Result.Failure<GetWarningByIdResponse>(WarningErrors.NotFound);

        return Result.Success(_mapper.Map<GetWarningByIdResponse>(warning));
    }
}
