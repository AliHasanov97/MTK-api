using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions;
using MTK.Modules.Buildings.Application.Abstractions.Data;
using MTK.Modules.Buildings.Domain.Appeals;

namespace MTK.Modules.Buildings.Application.Appeals.Commands.UpdateAppealStatus;

internal sealed class UpdateAppealStatusCommandHandler : ICommandHandler<UpdateAppealStatusCommand>
{
    private readonly IAppealRepository _appealRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAppealStatusCommandHandler(
        IAppealRepository appealRepository,
        ICurrentUserProvider currentUserProvider,
        IUnitOfWork unitOfWork)
    {
        _appealRepository = appealRepository;
        _currentUserProvider = currentUserProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateAppealStatusCommand request,
        CancellationToken cancellationToken)
    {
        var appeal = await _appealRepository.GetByIdDefaultAsync(request.AppealId, cancellationToken);

        if (appeal is null)
        {
            return Result.Failure(new Error("Appeal.NotFound", "Müraciət tapılmadı"));
        }

        appeal.UpdateStatus(request.NewStatus, _currentUserProvider.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
