using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Payroll.Commands.PayPayslip;

internal sealed class PayPayslipCommandHandler : ICommandHandler<PayPayslipCommand, PayrollRunResponse>
{
    private static readonly string[] AzMonthNames =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    private readonly IPayrollRunRepository _payrollRunRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public PayPayslipCommandHandler(
        IPayrollRunRepository payrollRunRepository,
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork)
    {
        _payrollRunRepository = payrollRunRepository;
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PayrollRunResponse>> Handle(PayPayslipCommand request, CancellationToken cancellationToken)
    {
        var run = await _payrollRunRepository.GetWithPayslipsAsync(request.PayrollRunId, cancellationToken);
        if (run is null)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.RunNotFound(request.PayrollRunId));
        }

        if (run.Status != PayrollRunStatus.Approved)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.NotApproved);
        }

        var payslip = run.Payslips.FirstOrDefault(p => p.EmployeeId == request.EmployeeId);
        if (payslip is null)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.EmployeeNotInRun(request.EmployeeId));
        }

        if (payslip.PaidAt.HasValue)
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.AlreadyPaid);
        }

        var paid = run.PayPayslip(request.EmployeeId, request.PaymentMethod, DateTimeOffset.UtcNow);

        var periodLabel = $"{AzMonthNames[run.Month - 1]} {run.Year}";
        _transactionRepository.Add(Transaction.ForPayrollPayment(paid, request.PaymentMethod, periodLabel));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
