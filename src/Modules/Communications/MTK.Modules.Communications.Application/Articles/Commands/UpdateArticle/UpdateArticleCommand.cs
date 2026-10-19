using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Commands.UpdateArticle;

public sealed record UpdateArticleCommand(
    Guid Id,
    string Title,
    string Summary,
    string Content,
    List<Guid>? FileAttachmentIds = null) : ICommand;
