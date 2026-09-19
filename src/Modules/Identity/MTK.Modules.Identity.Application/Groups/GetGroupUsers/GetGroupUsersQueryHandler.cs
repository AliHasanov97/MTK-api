using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Groups;

namespace MTK.Modules.Identity.Application.Groups.GetGroupUsers;

internal sealed class GetGroupUsersQueryHandler : IQueryHandler<GetGroupUsersQuery, List<UserDto>>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupUsersQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<Result<List<UserDto>>> Handle(GetGroupUsersQuery request, CancellationToken cancellationToken)
    {
        Group? group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group is null)
        {
            return Result.Failure<List<UserDto>>(GroupErrors.NotFound(request.GroupId));
        }

        var users = await _groupRepository.GetGroupMembersAsync(request.GroupId, cancellationToken);

        var userDtos = users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.PhoneNumber)).ToList();

        return Result.Success(userDtos);
    }
}
