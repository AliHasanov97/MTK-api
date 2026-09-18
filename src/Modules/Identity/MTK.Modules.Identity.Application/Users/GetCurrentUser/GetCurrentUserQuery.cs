using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetCurrentUser;

public sealed record GetCurrentUserQuery : IQuery<UserResponse>;
