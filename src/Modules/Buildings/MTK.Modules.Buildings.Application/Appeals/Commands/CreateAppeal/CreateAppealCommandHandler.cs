using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.CreateAppeal;

internal sealed class CreateAppealCommandHandler : ICommandHandler<CreateAppealCommand, Guid>
{
    private readonly IAppealRepository _appealRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAppealCommandHandler(
        IAppealRepository appealRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _appealRepository = appealRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateAppealCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserProvider.UserId;

        try
        {
            var appeal = Appeal.Create(
                request.Title,
                request.Description,
                request.Category,
                currentUserId,
                request.ApartmentId,
                request.GarageId);

            await _appealRepository.AddAsync(appeal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return appeal.Id;
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<Guid>(new Error("Appeal.ValidationError", ex.Message));
        }
    }
}
