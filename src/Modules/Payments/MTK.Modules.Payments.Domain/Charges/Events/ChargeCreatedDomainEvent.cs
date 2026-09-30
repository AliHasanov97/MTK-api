using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Parties;

namespace MTK.Modules.Payments.Domain.Charges.Events;

public sealed record ChargeCreatedDomainEvent(
    Guid ChargeId,
    PartyType PartyType,
    Guid PartyId,
    decimal Amount,
    string Period) : DomainEvent;
