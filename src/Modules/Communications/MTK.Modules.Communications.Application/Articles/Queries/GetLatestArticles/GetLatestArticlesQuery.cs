using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Queries.GetLatestArticles;

public sealed record GetLatestArticlesQuery(int Count = 5) : IQuery<List<ArticleResponse>>;
