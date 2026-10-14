using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Commands.CreateContract;

/// <summary>
/// Müqavilə tərəfi: <see cref="VendorId"/> (biz tədarükçüyə borcluyuq) və ya <see cref="CustomerId"/>
/// (müştəri bizə borcludur) — məhz biri dolu olmalıdır.
/// </summary>
public sealed record CreateContractCommand(
    string Number,
    Guid? VendorId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    Guid? CreatedByUserId = null,
    string? Note = null,
    Guid? CustomerId = null) : ICommand<Guid>;
