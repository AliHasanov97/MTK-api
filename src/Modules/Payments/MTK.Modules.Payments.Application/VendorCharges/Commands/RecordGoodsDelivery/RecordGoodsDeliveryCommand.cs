using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.VendorCharges.Commands.RecordGoodsDelivery;

/// <summary>
/// Mal tədarükünü (qaimə) qeydə alır: göndərilən miqdara görə tədarükçü
/// qarşısında borc yaradılır. Qiymət müqavilədəki mal sətrindən götürülür.
/// </summary>
public sealed record RecordGoodsDeliveryCommand(
    Guid ContractId,
    Guid GoodsItemId,
    decimal Quantity,
    string? Reference = null,
    DateTimeOffset? DeliveredOn = null) : ICommand<Guid>;
