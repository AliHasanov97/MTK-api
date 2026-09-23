using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

internal sealed class GetGroupUsersQueryHandler : IQueryHandler<GetGroupUsersQuery, List<UserDto>>
{
    private readonly IAuthenticationService _authenticationService;

    public GetGroupUsersQueryHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<List<UserDto>>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        List<UserDto> members = await _authenticationService.GetGroupMembersAsync(request.GroupId, cancellationToken);

        return Result.Success(members);
    }
}
