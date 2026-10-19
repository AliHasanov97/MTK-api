using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Communications.Domain.Articles.Events;

public sealed record ArticleCreatedDomainEvent(Guid ArticleId) : DomainEvent;