using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDetailResponse>;
