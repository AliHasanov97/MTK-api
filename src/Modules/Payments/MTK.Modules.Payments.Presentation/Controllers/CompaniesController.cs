using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Payments.Application.Companies.Commands.UpsertCompany;
using MTK.Modules.Payments.Application.Companies.Queries.GetCompany;

namespace MTK.Modules.Payments.Presentation.Controllers;

public class CompaniesController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCompanyQuery(), cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Giriş etməmiş istifadəçilər üçün (landing səhifəsi): yalnız şirkət adı.
    /// Direktor və ünvan bilərəkdən qaytarılmır.
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublic(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCompanyQuery(), cancellationToken);
        return result.IsSuccess ? Success(new { result.Value.Name }) : BadRequest(result.Error);
    }

    [HttpPut]
    [RequireAnyRole(Roles.Admin)]
    public async Task<IActionResult> Upsert([FromBody] UpsertCompanyCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess
            ? Success(result.Value, "Şirkət profili yeniləndi")
            : BadRequest(result.Error);
    }
}
