using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Commands.DeleteArticle;

public sealed record DeleteArticleCommand(Guid Id) : ICommand;
