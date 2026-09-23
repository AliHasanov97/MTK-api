namespace MTK.Modules.Identity.Application.Abstractions;

public sealed record GroupDto(
    Guid Id,
    string Name,
    string? Description);
