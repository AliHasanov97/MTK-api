using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.UnexcusedAbsences;
using MTK.Modules.Hr.Domain.UnexcusedAbsences;

namespace MTK.Modules.Hr.Application.UnexcusedAbsences.DeleteUnexcusedAbsence;

internal sealed class DeleteUnexcusedAbsenceCommandHandler : ICommandHandler<DeleteUnexcusedAbsenceCommand>
{
    private readonly IUnexcusedAbsenceRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUnexcusedAbsenceCommandHandler(
        IUnexcusedAbsenceRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUnexcusedAbsenceCommand request, CancellationToken cancellationToken)
    {
        var absence = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (absence is null)
            return Result.Failure(UnexcusedAbsenceErrors.NotFound);

        await _repository.DeleteAsync(absence, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
