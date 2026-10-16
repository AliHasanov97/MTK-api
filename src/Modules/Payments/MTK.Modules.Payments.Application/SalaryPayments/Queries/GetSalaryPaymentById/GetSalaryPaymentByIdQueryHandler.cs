using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.GetSalaryPaymentById;

internal sealed class GetSalaryPaymentByIdQueryHandler
    : IQueryHandler<GetSalaryPaymentByIdQuery, SalaryPaymentResponse>
{
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;

    public GetSalaryPaymentByIdQueryHandler(ISalaryPaymentRepository salaryPaymentRepository)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
    }

    public async Task<Result<SalaryPaymentResponse>> Handle(
        GetSalaryPaymentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var salaryPayment = await _salaryPaymentRepository.GetByIdWithLinesAsync(
            request.SalaryPaymentId, cancellationToken);

        if (salaryPayment is null)
        {
            return Result.Failure<SalaryPaymentResponse>(SalaryPaymentErrors.PaymentNotFound);
        }

        return Result.Success(new SalaryPaymentResponse(
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
            salaryPayment.UpdatedAt));
    }
}
