using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.SearchGroups;

internal sealed class SearchGroupsQueryHandler : IQueryHandler<SearchGroupsQuery, SearchGroupsResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public SearchGroupsQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<Result<SearchGroupsResponse>> Handle(SearchGroupsQuery request, CancellationToken cancellationToken)
    {
        // Keycloak does not provide a direct method to search groups via IAuthenticationService
        // This requires additional implementation in the service
        return Task.FromResult(Result.Failure<SearchGroupsResponse>(
            new Error("Group.SearchNotSupported",
                "Group search is not supported with current Keycloak integration. Additional API methods required.")));
    }
}
