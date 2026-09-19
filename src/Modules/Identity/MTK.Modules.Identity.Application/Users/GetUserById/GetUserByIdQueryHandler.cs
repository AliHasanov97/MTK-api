using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Domain.Users;

namespace MTK.Modules.Identity.Application.Users.GetUserById;

internal sealed class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserDetailResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDetailResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDetailResponse>(UserErrors.NotFound(request.Id));
        }

        var response = new UserDetailResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.Status.ToString(),
            user.IdentityId,
            user.CreatedAt,
            user.UpdatedAt);

        return Result.Success(response);
    }
}
