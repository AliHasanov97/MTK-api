using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetAllUsers;

public sealed record GetAllUsersQuery() : IQuery<List<UserResponse>>;
