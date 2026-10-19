using MTK.Common.Application.Messaging;

namespace MTK.Modules.Communications.Application.Articles.Commands.CreateArticle;

public sealed record CreateArticleCommand(
    string Title,
    string Summary,
    string Content,
    List<Guid>? FileAttachmentIds = null) : ICommand<CreateArticleResponse>;
