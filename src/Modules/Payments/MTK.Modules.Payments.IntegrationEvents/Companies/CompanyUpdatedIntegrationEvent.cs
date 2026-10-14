using MTK.Common.Application.EventBus;

namespace MTK.Modules.Payments.IntegrationEvents.Companies;

/// <summary>Şirkət profili yeniləndikdə digər modullara bildiriş — snapshot-lar sinxronlaşır.</summary>
public sealed class CompanyUpdatedIntegrationEvent : IntegrationEvent
{
    public CompanyUpdatedIntegrationEvent(
        Guid integrationEventId,
        DateTime occurredOnUtc,
        Guid companyId,
        string name,
        string? director,
        string address)
        : base(integrationEventId, occurredOnUtc)
    {
        CompanyId = companyId;
        Name = name;
        Director = director;
        Address = address;
    }

    public Guid CompanyId { get; }
    public string Name { get; }
    public string? Director { get; }
    public string Address { get; }
}
