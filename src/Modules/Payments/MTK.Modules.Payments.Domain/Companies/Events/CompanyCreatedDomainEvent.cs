using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Companies.Events;

public sealed record CompanyCreatedDomainEvent : DomainEvent
{
    public Guid CompanyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Director { get; init; }
    public string Address { get; init; } = string.Empty;
}
