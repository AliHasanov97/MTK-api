using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.CreatePayrollTaxBracket;
using MTK.Modules.Payments.Application.PayrollTaxBrackets.Commands.DeactivatePayrollTaxBracket;
using MTK.Modules.Payments.Application.PayrollTaxBrackets.Queries.GetPayrollTaxBrackets;

namespace MTK.Modules.Payments.Presentation.Controllers;

/// <summary>
/// DSMF/işsizlik/tibbi sığorta/gəlir vergisi dərəcə pillələri — qanun dəyişəndə HR/mühasibatlıq
/// burdan yeni pillə əlavə edir (köhnəsi avtomatik bağlanır), kod/deploy lazım olmadan.
/// </summary>
[RequireAnyRole(Roles.Admin, Roles.Accountant)]
public class PayrollTaxBracketsController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPayrollTaxBracketsQuery(), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePayrollTaxBracketCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Success(result.Value, "Dərəcə pilləsi yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id,
        [FromBody] DeactivatePayrollTaxBracketRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeactivatePayrollTaxBracketCommand(id, request.EffectiveTo), cancellationToken);
        return result.IsSuccess
            ? Success("Dərəcə pilləsi bağlandı")
            : BadRequest(result.Error);
    }
}

public sealed record DeactivatePayrollTaxBracketRequest(DateTimeOffset EffectiveTo);
