using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.SetContractServiceStatus;

/// <summary>
/// Tək xidməti dayandırır/bərpa edir — müqavilə aktiv qalır, sadəcə həmin
/// xidmətdən artıq borc yaranmır.
/// </summary>
public sealed record SetContractServiceStatusCommand(
    Guid ContractId,
    Guid ServiceId,
    bool IsActive) : ICommand;
