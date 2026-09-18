using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Identity.Application.Abstractions;

namespace MTK.Modules.Identity.Application.Users.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, UserResponse>
{
    private readonly IUserContext _userContext;

    public GetCurrentUserQueryHandler(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public Task<Result<UserResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var response = new UserResponse(
            _userContext.UserId,
            _userContext.Email,
            _userContext.FirstName,
            _userContext.LastName,
            _userContext.PhoneNumber,
            _userContext.Role);

        return Task.FromResult(Result.Success(response));
    }
}
