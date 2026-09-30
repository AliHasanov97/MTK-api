using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.ChangeContractStatus;

/// <summary>Müqaviləni aktivləşdirir (ən azı bir xidmət tələb olunur).</summary>
public sealed record ActivateContractCommand(Guid ContractId) : ICommand;

/// <summary>Müqaviləni dayandırır — aktiv müqavilə tələb olunur.</summary>
public sealed record SuspendContractCommand(
    Guid ContractId,
    string? Note = null) : ICommand;

/// <summary>Müqaviləni vaxtından əvvəl ləğv edir və bitmə tarixini yeniləyir.</summary>
public sealed record TerminateContractCommand(
    Guid ContractId,
    DateTimeOffset TerminatedOn,
    string? Note = null) : ICommand;

/// <summary>Soft delete. Aktiv müqavilə əvvəlcə ləğv edilməlidir.</summary>
public sealed record DeleteContractCommand(Guid ContractId) : ICommand;
