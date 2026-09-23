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

    public async Task<Result<GroupDetailResponse>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        Abstractions.GroupDto? group = await _authenticationService.GetGroupByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure<GroupDetailResponse>(
                new Error("Group.NotFound", $"Group with ID '{request.Id}' was not found."));
        }

        var response = new GroupDetailResponse(
            group.Id,
            group.Id, // KeycloakGroupId is same as Id from Keycloak
            group.Name,
            group.Description,
            null, // ParentGroupId - not provided by Keycloak API in this method
            DateTime.UtcNow, // CreatedAt - not provided by Keycloak API
            null); // UpdatedAt - not provided by Keycloak API

        return Result.Success(response);
    }
}
