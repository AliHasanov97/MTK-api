using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;

public sealed record CreateContractCommand(
    string Number,
    Guid VendorId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid? CreatedByUserId = null,
    string? Note = null,
    string Currency = "AZN") : ICommand<Guid>;
