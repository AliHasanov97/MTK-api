using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.SearchUnpaidLeaveApplications;

internal sealed class SearchUnpaidLeaveApplicationsQueryHandler
    : IQueryHandler<SearchUnpaidLeaveApplicationsQuery, SearchUnpaidLeaveApplicationsResponse>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly IMapper _mapper;

    public SearchUnpaidLeaveApplicationsQueryHandler(
        IUnpaidLeaveApplicationRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SearchUnpaidLeaveApplicationsResponse>> Handle(
        SearchUnpaidLeaveApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _repository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _repository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var data = _mapper.Map<List<SearchUnpaidLeaveApplicationsResponseItem>>(applications);
        return Result.Success(new SearchUnpaidLeaveApplicationsResponse(data, totalCount, request.Page, request.PageSize));
    }
}
