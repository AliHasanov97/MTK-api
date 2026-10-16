using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.SalaryPayments.Events;
using MTK.Modules.Payments.Domain.Transactions;

namespace MTK.Modules.Payments.Application.SalaryPayments.Events;

/// <summary>
/// Maaş ödənişi Paid statusuna keçdikdə Transaction-lar yaradır.
/// Hər line üçün ayrıca transaction - detallı mühasibat qeydi.
/// </summary>
internal sealed class SalaryPaymentPaidDomainEventHandler
    : DomainEventHandler<SalaryPaymentPaidDomainEvent>
{
    private static readonly string[] AzMonthNames =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public SalaryPaymentPaidDomainEventHandler(
        ISalaryPaymentRepository salaryPaymentRepository,
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        SalaryPaymentPaidDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch SalaryPayment with lines
        var salaryPayment = await _salaryPaymentRepository.GetByIdWithLinesAsync(
            domainEvent.SalaryPaymentId, cancellationToken);

        if (salaryPayment is null)
        {
            throw new InvalidOperationException(
                $"SalaryPayment {domainEvent.SalaryPaymentId} tapılmadı");
        }

        // 2. Build period label
        var periodLabel = $"{AzMonthNames[salaryPayment.SalaryMonth - 1]} {salaryPayment.SalaryYear}";

        // 3. Create transactions for each line
        var transactions = Transaction.ForSalaryPayment(salaryPayment, periodLabel);

        foreach (var transaction in transactions)
        {
            _transactionRepository.Add(transaction);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Recalculate company balance
        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
