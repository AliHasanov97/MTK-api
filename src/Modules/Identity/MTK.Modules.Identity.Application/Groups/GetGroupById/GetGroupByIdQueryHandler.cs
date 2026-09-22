using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

internal sealed class GetGroupByIdQueryHandler : IQueryHandler<GetGroupByIdQuery, GroupDetailResponse>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupByIdQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public Task<Result<GroupDetailResponse>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        // Keycloak does not provide a direct method to get group by ID via IAuthenticationService
        // This requires additional implementation in the service
        return Task.FromResult(Result.Failure<GroupDetailResponse>(
            new Error("Group.GetByIdNotSupported",
                "Group lookup by ID is not supported with current Keycloak integration. Additional API methods required.")));
    }
}
