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

    public async Task<Result<List<GroupResponse>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _authenticationService.GetAllGroupsAsync(cancellationToken);

        var response = groups.Select(g => new GroupResponse(
            g.Id,
            g.Name,
            g.Description,
            g.ParentId
        )).ToList();

        return Result.Success(response);
    }
}
