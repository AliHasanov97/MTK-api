using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Domain.Articles.Events;

public sealed record ArticlePublishedDomainEvent(Guid ArticleId) : DomainEvent;