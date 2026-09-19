using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.GetGroups;

internal sealed class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, List<GroupResponse>>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Result<List<GroupResponse>>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetAllAsync(cancellationToken);

        var response = groups.Select(g => new GroupResponse(
            g.Id,
            g.KeycloakGroupId,
            g.Name,
            g.Description,
            g.ParentGroupId)).ToList();

        return Result.Success(response);
    }
}
