using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroups;

internal sealed class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, List<GroupResponse>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupsQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<Result<List<GroupResponse>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        // Keycloak does not provide a direct method to list all groups via IAuthenticationService
        // This requires additional implementation in the service
        return Task.FromResult(Result.Failure<List<GroupResponse>>(
            new Error("Group.ListNotSupported",
                "Group listing is not supported with current Keycloak integration. Additional API methods required.")));
    }
}
