using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Warnings;

namespace MTK.Modules.Hr.Application.Warnings.DeleteWarning;

internal sealed class DeleteWarningCommandHandler : ICommandHandler<DeleteWarningCommand>
{
    private readonly IWarningRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWarningCommandHandler(
        IWarningRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteWarningCommand request, CancellationToken cancellationToken)
    {
        var warning = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (warning is null)
            return Result.Failure(WarningErrors.NotFound);

        await _repository.DeleteAsync(warning, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
