using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Contracts.Events;

public sealed record ContractTerminatedDomainEvent(
    Guid ContractId,
    Guid PartyId,
    string Number,
    DateTimeOffset TerminatedOn) : DomainEvent;
