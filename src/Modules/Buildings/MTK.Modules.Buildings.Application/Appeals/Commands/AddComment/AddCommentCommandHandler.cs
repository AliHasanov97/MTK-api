using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.AddComment;

internal sealed class AddCommentCommandHandler : ICommandHandler<AddCommentCommand, Guid>
{
    private readonly IAppealRepository _appealRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public AddCommentCommandHandler(
        IAppealRepository appealRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _appealRepository = appealRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        var appeal = await _appealRepository.GetWithCommentsAsync(request.AppealId, cancellationToken);

        if (appeal is null)
        {
            return Result.Failure<Guid>(new Error("Appeal.NotFound", "Müraciət tapılmadı"));
        }

        try
        {
            var comment = appeal.AddComment(request.Text, _currentUserProvider.UserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return comment.Id;
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(new Error("Appeal.ValidationError", ex.Message));
        }
    }
}
