using MediatR;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Buildings.Application.Appeals.Commands.AddComment;
using MTK.Modules.Buildings.Application.Appeals.Commands.CreateAppeal;
using MTK.Modules.Buildings.Application.Appeals.Commands.UpdateAppealStatus;
using MTK.Modules.Buildings.Application.Appeals.Queries.GetAppealById;
using MTK.Modules.Buildings.Application.Appeals.Queries.SearchAppeals;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Presentation.Controllers;

[Route("api/appeals")]
public class AppealsController(ISender sender) : BaseController(sender)
{
    /// <summary>
    /// Yeni müraciət yarat (istifadəçi).
    /// </summary>
    [HttpPost]
    [RequireAnyRole(Roles.Owner, Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> CreateAppeal(
        [FromBody] CreateAppealRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAppealCommand(
            request.Title,
            request.Description,
            request.Category,
            request.ApartmentId,
            request.GarageId);

        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Created($"/api/appeals/{result.Value}", result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Müraciət axtar (filter/sort/pagination ilə).
    /// Admin bütün müraciətləri, istifadəçi yalnız öz müraciətlərini görür.
    /// </summary>
    [HttpPost("search")]
    [RequireAnyRole(Roles.Owner, Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> SearchAppeals(
        [FromBody] SearchAppealsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Müraciəti ID ilə gətir (detaylı baxış).
    /// </summary>
    [HttpGet("{id:guid}")]
    [RequireAnyRole(Roles.Owner, Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> GetAppealById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAppealByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Müraciətin statusunu dəyiş (admin).
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> UpdateAppealStatus(
        Guid id,
        [FromBody] UpdateAppealStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAppealStatusCommand(id, request.NewStatus);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success("Status dəyişdirildi")
            : BadRequest(result.Error);
    }

    /// <summary>
    /// Müraciətə şərh əlavə et (hər iki tərəf).
    /// </summary>
    [HttpPost("{id:guid}/comments")]
    [RequireAnyRole(Roles.Owner, Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> AddComment(
        Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand(id, request.Text);
        var result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Success(result.Value)
            : BadRequest(result.Error);
    }
}

// Request DTOs
public sealed record CreateAppealRequest(
    string Title,
    string Description,
    AppealCategory Category,
    Guid? ApartmentId,
    Guid? GarageId);

public sealed record UpdateAppealStatusRequest(AppealStatus NewStatus);

public sealed record AddCommentRequest(string Text);
