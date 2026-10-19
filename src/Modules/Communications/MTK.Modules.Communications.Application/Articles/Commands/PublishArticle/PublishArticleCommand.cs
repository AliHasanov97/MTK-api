using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Commands.PublishArticle;

public sealed record PublishArticleCommand(Guid Id) : ICommand;
