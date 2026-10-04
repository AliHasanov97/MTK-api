using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Vendors.Commands.CreateVendor;
using MTK.Modules.Payments.Application.Vendors.Commands.DeleteVendor;
using MTK.Modules.Payments.Application.Vendors.Commands.SetVendorStatus;
using MTK.Modules.Payments.Application.Vendors.Commands.UpdateVendor;
using MTK.Modules.Payments.Application.Vendors.Queries.GetVendorById;
using MTK.Modules.Payments.Application.Vendors.Queries.SearchVendors;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class VendorsController(ISender sender) : BaseController(sender)
{
    [HttpPost("search")]
    public async Task<IActionResult> SearchVendors(
        [FromBody] SearchVendorsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{vendorId:guid}")]
    public async Task<IActionResult> GetVendorById(
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetVendorByIdQuery(vendorId), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateVendor(
        [FromBody] CreateVendorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Tədarükçü uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{vendorId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateVendor(
        Guid vendorId,
        [FromBody] UpdateVendorCommand command,
        CancellationToken cancellationToken)
    {
        // Route identifikatoru gövdədəkindən üstündür.
        var result = await _sender.Send(command with { VendorId = vendorId }, cancellationToken);

        return result.IsSuccess
            ? Success("Tədarükçü uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpPut("{vendorId:guid}/status")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> SetVendorStatus(
        Guid vendorId,
        [FromBody] SetVendorStatusCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { VendorId = vendorId }, cancellationToken);

        return result.IsSuccess
            ? Success(command.IsActive ? "Tədarükçü aktivləşdirildi" : "Tədarükçü dayandırıldı")
            : BadRequest(result.Error);
    }

    [HttpDelete("{vendorId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> DeleteVendor(
        Guid vendorId,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteVendorCommand(vendorId), cancellationToken);

        return result.IsSuccess
            ? Success("Tədarükçü silindi")
            : BadRequest(result.Error);
    }
}
