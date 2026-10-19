using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Queries.GetArticleById;

public sealed record GetArticleByIdQuery(Guid Id) : IQuery<ArticleResponse>;
