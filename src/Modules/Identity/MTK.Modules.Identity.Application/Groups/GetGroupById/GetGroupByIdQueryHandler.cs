using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.GetGroupById;

internal sealed class GetGroupByIdQueryHandler : IQueryHandler<GetGroupByIdQuery, GroupDetailResponse>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupByIdQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Result<GroupDetailResponse>> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure<GroupDetailResponse>(GroupErrors.NotFound(request.Id));
        }

        var response = new GroupDetailResponse(
            group.Id,
            group.KeycloakGroupId,
            group.Name,
            group.Description,
            group.ParentGroupId,
            group.CreatedAt,
            group.UpdatedAt);

        return Result.Success(response);
    }
}
