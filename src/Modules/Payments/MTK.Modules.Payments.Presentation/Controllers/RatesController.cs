using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Rates.Commands.CreateRate;
using MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class RatesController(ISender sender) : BaseController(sender)
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRates(CancellationToken cancellationToken)
    {
        var query = new GetCurrentRatesQuery();
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateRate(
        [FromBody] CreateRateCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Tarif uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{rateId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateRate(
        Guid rateId,
        [FromBody] UpdateRateCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { RateId = rateId }, cancellationToken);

        return result.IsSuccess
            ? Success("Tarif uğurla yeniləndi")
            : BadRequest(result.Error);
    }
}
