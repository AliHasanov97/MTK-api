using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Application.Payroll;
using MTK.Modules.Payments.Domain.Payroll;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.Payroll.Commands.PayPayrollRun;

internal sealed class PayPayrollRunCommandHandler : ICommandHandler<PayPayrollRunCommand, PayrollRunResponse>
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

    public PayPayrollRunCommandHandler(
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

    public async Task<Result<PayrollRunResponse>> Handle(PayPayrollRunCommand request, CancellationToken cancellationToken)
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

        if (run.Payslips.All(p => p.PaidAt.HasValue))
        {
            return Result.Failure<PayrollRunResponse>(PayrollErrors.NothingToPay);
        }

        var paid = run.PayAllPayslips(request.PaymentMethod, DateTimeOffset.UtcNow);

        var periodLabel = $"{AzMonthNames[run.Month - 1]} {run.Year}";
        foreach (var payslip in paid)
        {
            _transactionRepository.Add(Transaction.ForPayrollPayment(payslip, request.PaymentMethod, periodLabel));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PayrollRunResponse.FromDomain(run));
    }
}
