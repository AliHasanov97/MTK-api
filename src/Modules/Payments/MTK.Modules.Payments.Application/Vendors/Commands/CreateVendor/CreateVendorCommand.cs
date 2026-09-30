using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.Modules.Payments.Application.Vendors.Commands.CreateVendor;

public sealed record CreateVendorCommand(
    string Name,
    VendorType VendorType,
    string? Voen = null,
    string? Director = null,
    string? Email = null,
    string? Phone = null,
    string? Address = null,
    string? Note = null) : ICommand<Guid>;
