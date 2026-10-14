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

        // Generic repo.DeleteAsync set-DeletedAt-dən keçmir ki, domain event raise olunsun
        // (Payments-ə SalaryDeductionDeleted sync-i üçün lazımdır) — ona görə domain metodu çağırılır.
        salaryDeduction.Delete();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
