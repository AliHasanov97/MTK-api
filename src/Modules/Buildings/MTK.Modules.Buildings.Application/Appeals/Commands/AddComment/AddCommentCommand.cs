using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.AddComment;

public sealed record AddCommentCommand(
    Guid AppealId,
    string Text) : ICommand<Guid>;
