using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Vendors.Commands.SetVendorStatus;

/// <summary>
/// Tədarükçünü aktivləşdirir/dayandırır. Müqaviləsi olan tədarükçünü silmək
/// əvəzinə dayandırmaq tövsiyə olunur.
/// </summary>
public sealed record SetVendorStatusCommand(
    Guid VendorId,
    bool IsActive) : ICommand;
