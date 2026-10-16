using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments;

namespace MTK.Modules.Payments.Application.SalaryPayments.Commands.PaySalaryPayment;

internal sealed class PaySalaryPaymentCommandHandler
    : ICommandHandler<PaySalaryPaymentCommand, SalaryPaymentResponse>
{
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaySalaryPaymentCommandHandler(
        ISalaryPaymentRepository salaryPaymentRepository,
        IUnitOfWork unitOfWork)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SalaryPaymentResponse>> Handle(
        PaySalaryPaymentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Fetch SalaryPayment with lines
        var salaryPayment = await _salaryPaymentRepository.GetByIdWithLinesAsync(
            request.SalaryPaymentId, cancellationToken);

        if (salaryPayment is null)
        {
            return Result.Failure<SalaryPaymentResponse>(SalaryPaymentErrors.PaymentNotFound);
        }

        // 2. Mark as paid (raises SalaryPaymentPaidDomainEvent)
        try
        {
            salaryPayment.MarkAsPaid();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<SalaryPaymentResponse>(new Error(
                "SalaryPayment.InvalidOperation",
                ex.Message));
        }

        // 3. Save changes (domain event handler will create transactions)
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Return response
        return Result.Success(MapToResponse(salaryPayment));
    }

    private static SalaryPaymentResponse MapToResponse(SalaryPayment salaryPayment)
    {
        return new SalaryPaymentResponse(
            salaryPayment.Id,
            salaryPayment.EmployeeId,
            salaryPayment.EmployeeFullName,
            salaryPayment.SalaryMonth,
            salaryPayment.SalaryYear,
            salaryPayment.BaseSalary,
            salaryPayment.TotalBonuses,
            salaryPayment.TotalAdjustments,
            salaryPayment.GrossSalary,
            salaryPayment.PaymentMethod,
            salaryPayment.PaymentDate,
            salaryPayment.Status,
            salaryPayment.Notes,
            salaryPayment.Lines.Select(l => new SalaryPaymentLineResponse(
                l.Id,
                l.LineType,
                l.Description,
                l.Amount,
                l.SourceBonusId,
                l.SourceAdjustmentId)).ToList(),
            salaryPayment.CreatedAt,
            salaryPayment.UpdatedAt);
    }
}
