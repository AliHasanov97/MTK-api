using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

namespace MTK.Modules.Hr.Application.NoticesOfChangeInWorkingConditions.GetNoticeOfChangeInWorkingConditionsById;

internal sealed class GetNoticeOfChangeInWorkingConditionsByIdQueryHandler : IQueryHandler<GetNoticeOfChangeInWorkingConditionsByIdQuery, GetNoticeOfChangeInWorkingConditionsByIdResponse>
{
    private readonly INoticeOfChangeInWorkingConditionsRepository _repository;
    private readonly IMapper _mapper;

    public GetNoticeOfChangeInWorkingConditionsByIdQueryHandler(
        INoticeOfChangeInWorkingConditionsRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<GetNoticeOfChangeInWorkingConditionsByIdResponse>> Handle(GetNoticeOfChangeInWorkingConditionsByIdQuery request, CancellationToken cancellationToken)
    {
        var notice = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (notice is null)
            return Result.Failure<GetNoticeOfChangeInWorkingConditionsByIdResponse>(NoticeOfChangeInWorkingConditionsErrors.NotFound);

        return Result.Success(_mapper.Map<GetNoticeOfChangeInWorkingConditionsByIdResponse>(notice));
    }
}