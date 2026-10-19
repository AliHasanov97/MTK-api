using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MTK.Common.Application.Authorization;
using MTK.Modules.Communications.Application.Articles.Commands.CreateArticle;
using MTK.Modules.Communications.Application.Articles.Commands.DeleteArticle;
using MTK.Modules.Communications.Application.Articles.Commands.PublishArticle;
using MTK.Modules.Communications.Application.Articles.Commands.UpdateArticle;
using MTK.Modules.Communications.Application.Articles.Queries.GetArticleById;
using MTK.Modules.Communications.Application.Articles.Queries.SearchArticles;
using MTK.Modules.Communications.Application.Articles.Queries.GetLatestArticles;

namespace MTK.Modules.Communications.Presentation.Controllers;

public class ArticlesController : BaseController
{
    public ArticlesController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Son yayımlanan xəbərləri gətir (Landing page üçün - Public)
    /// </summary>
    [HttpGet("latest")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLatest([FromQuery] int count = 5, CancellationToken cancellationToken = default)
    {
        var query = new GetLatestArticlesQuery(count);
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Məqaləni ID-yə görə gətir (Public)
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetArticleByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : NotFound(result.Error);
    }

    /// <summary>
    /// Məqalələri axtar (filtering, sorting, pagination ilə) (Admin və Komandant)
    /// </summary>
    [HttpPost("search")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Search(
        [FromBody] SearchArticlesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return result.IsSuccess ? Success(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Yeni məqalə yarat (Admin və Komandant)
    /// </summary>
    [HttpPost]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Create([FromBody] CreateArticleCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success(result.Value, "Məqalə uğurla yaradıldı") : BadRequest(result.Error);
    }

    /// <summary>
    /// Məqaləni redaktə et (Admin və Komandant)
    /// </summary>
    [HttpPut("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new { error = "ID uyğunsuzluğu" });
        }

        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success("Məqalə uğurla yeniləndi") : BadRequest(result.Error);
    }

    /// <summary>
    /// Məqaləni yayımla (Admin və Komandant)
    /// </summary>
    [HttpPost("{id:guid}/publish")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var command = new PublishArticleCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success("Məqalə uğurla yayımlandı") : BadRequest(result.Error);
    }

    /// <summary>
    /// Məqaləni sil (Admin və Komandant)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [RequireAnyRole(Roles.Admin, Roles.BuildingManager)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteArticleCommand(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Success("Məqalə uğurla silindi") : BadRequest(result.Error);
    }
}
