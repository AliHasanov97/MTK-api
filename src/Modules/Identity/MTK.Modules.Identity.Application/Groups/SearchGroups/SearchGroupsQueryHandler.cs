using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

internal sealed class SearchGroupsQueryHandler : IQueryHandler<SearchGroupsQuery, SearchGroupsResponse>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public SearchGroupsQueryHandler(IAuthenticationService authenticationService, IMapper mapper)
    {
        _authenticationService = authenticationService;
        _mapper = mapper;
    }

    public async Task<Result<SearchGroupsResponse>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        List<GroupDto> allGroups = await _authenticationService.SearchGroupsAsync(request.SearchTerm, cancellationToken);

        int totalCount = allGroups.Count;

        // Apply pagination
        List<GroupDto> paginatedGroups = allGroups
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var groupResults = _mapper.Map<List<GroupSearchResult>>(paginatedGroups);

        var response = new SearchGroupsResponse(
            groupResults,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(response);
    }
}
