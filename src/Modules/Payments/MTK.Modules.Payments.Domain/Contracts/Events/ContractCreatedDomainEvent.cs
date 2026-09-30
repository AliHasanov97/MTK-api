using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Contracts.Events;

public sealed record ContractCreatedDomainEvent(
    Guid ContractId,
    Guid VendorId,
    string Number) : DomainEvent;
