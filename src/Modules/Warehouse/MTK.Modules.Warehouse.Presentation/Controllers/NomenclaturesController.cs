using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Warehouse.Application.Nomenclatures.Commands.CreateNomenclature;
using MTK.Modules.Warehouse.Application.Nomenclatures.Commands.DeleteNomenclature;
using MTK.Modules.Warehouse.Application.Nomenclatures.Commands.UpdateNomenclature;
using MTK.Modules.Warehouse.Application.Nomenclatures.Queries.GetAllNomenclatures;
using MTK.Modules.Warehouse.Application.Nomenclatures.Queries.GetNomenclatureById;
using MTK.Modules.Warehouse.Application.Nomenclatures.Queries.SearchNomenclatures;

namespace MTK.Modules.Warehouse.Presentation.Controllers;

public class NomenclaturesController(ISender sender) : BaseController(sender)
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAllNomenclaturesQuery(pageNumber, pageSize, isActive);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetNomenclatureByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] SearchNomenclaturesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Create(
        [FromBody] CreateNomenclatureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value, "Nomenklatura uğurla yaradıldı")
            : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateNomenclatureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command with { Id = id }, cancellationToken);

        return result.IsSuccess
            ? Success("Nomenklatura uğurla yeniləndi")
            : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteNomenclatureCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Nomenklatura uğurla silindi")
            : BadRequest(result.Error);
    }
}
