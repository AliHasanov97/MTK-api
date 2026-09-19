using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetAllUsers;

internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, List<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<List<UserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User> users = await _userRepository.GetAllAsync(cancellationToken);

        var response = users.Select(u => new UserResponse(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.PhoneNumber,
            u.Status.ToString()))
            .ToList();

        return Result.Success(response);
    }
}
