using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.SalaryDeductions;

namespace MTK.Modules.Hr.Application.SalaryDeductions.DeleteSalaryDeduction;

internal sealed class DeleteSalaryDeductionCommandHandler : ICommandHandler<DeleteSalaryDeductionCommand>
{
    private readonly ISalaryDeductionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSalaryDeductionCommandHandler(
        ISalaryDeductionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSalaryDeductionCommand request, CancellationToken cancellationToken)
    {
        var salaryDeduction = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (salaryDeduction is null)
            return Result.Failure(SalaryDeductionErrors.NotFound);

        await _repository.DeleteAsync(salaryDeduction, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
