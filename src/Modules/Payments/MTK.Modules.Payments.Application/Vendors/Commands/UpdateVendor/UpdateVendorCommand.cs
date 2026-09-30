using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application.Vendors.Commands.UpdateVendor;

public sealed record UpdateVendorCommand(
    Guid VendorId,
    string Name,
    VendorType VendorType,
    string? Voen = null,
    string? Director = null,
    string? Email = null,
    string? Phone = null,
    string? Address = null,
    string? Note = null) : ICommand;
