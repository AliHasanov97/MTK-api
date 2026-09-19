using MTK.Common.Application.Messaging;

namespace MTK.Modules.Identity.Application.Profile.ChangeUserInfo;

public sealed record ChangeUserInfoCommand(
    string? Password,
    string? PhoneNumber) : ICommand;
