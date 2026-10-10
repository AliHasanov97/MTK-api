using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;

namespace MTK.Modules.Hr.Application.Applications.SearchApplications;

internal sealed class SearchApplicationsQueryHandler(
    IApplicationRepository repository,
    IMapper mapper)
    : IQueryHandler<SearchApplicationsQuery, SearchApplicationsResponse>
{
    public async Task<Result<SearchApplicationsResponse>> Handle(
        SearchApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var applications = await repository.SearchAsync(
                request.Filters,
                request.SortCriteria,
                request.SearchTerm,
                request.Page,
                request.PageSize,
                cancellationToken);

            var totalCount = await repository.CountAsync(
                request.Filters,
                request.SortCriteria,
                request.SearchTerm,
                cancellationToken);

            var data = mapper.Map<List<SearchApplicationsResponseItem>>(applications);

            return Result.Success(new SearchApplicationsResponse(
                data,
                totalCount,
                request.Page ?? 1,
                request.PageSize ?? 10));
        }
        catch (Exception ex)
        {
         
            throw;
        }
    }
}