namespace MTK.Modules.Communications.Application.Articles.Commands.CreateArticle;

public sealed record CreateArticleResponse(
    Guid Id,
    string Title,
    string Summary,
    string Content,
    string Status,
    DateTimeOffset CreatedAt);
