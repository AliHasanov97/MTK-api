using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Rates.Commands.CreateRate;
using MTK.Modules.Payments.Application.Rates.Commands.UpdateRate;
using MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;

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
        [FromBody] CreateRateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRateCommand(
            request.RateType,
            request.Amount,
            request.EffectiveFrom,
            request.Description,
            request.GarageType);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Tarif uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{rateId:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateRate(
        Guid rateId,
        [FromBody] UpdateRateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRateCommand(
            rateId,
            request.Amount,
            request.Description);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Tarif uğurla yeniləndi")
            : BadRequest(result.Error);
    }
}

public sealed record CreateRateRequest(
    RateType RateType,
    decimal Amount,
    DateTimeOffset EffectiveFrom,
    string? Description,
    GarageType? GarageType = null);

public sealed record UpdateRateRequest(
    decimal Amount,
    string? Description);
