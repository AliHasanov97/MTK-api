using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Vendors.Commands.DeleteVendor;

public sealed record DeleteVendorCommand(Guid VendorId) : ICommand;
