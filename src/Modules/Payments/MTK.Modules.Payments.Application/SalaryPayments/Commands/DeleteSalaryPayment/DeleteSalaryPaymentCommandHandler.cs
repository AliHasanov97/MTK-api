using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.DeleteSalaryPayment;

internal sealed class DeleteSalaryPaymentCommandHandler : ICommandHandler<DeleteSalaryPaymentCommand>
{
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSalaryPaymentCommandHandler(
        ISalaryPaymentRepository salaryPaymentRepository,
        IUnitOfWork unitOfWork)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteSalaryPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var salaryPayment = await _salaryPaymentRepository.GetByIdAsync(
            request.SalaryPaymentId, cancellationToken);

        if (salaryPayment is null)
        {
            return Result.Failure(SalaryPaymentErrors.PaymentNotFound);
        }

        if (salaryPayment.Status != SalaryPaymentStatus.Draft)
        {
            return Result.Failure(SalaryPaymentErrors.PaymentCannotBeDeleted);
        }

        salaryPayment.SetDeletedAt();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
