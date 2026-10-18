using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.CompanyBalances.Services;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Domain.VacationPays.Events;

namespace MTK.Modules.Payments.Application.VacationPays.Events;

/// <summary>
/// Məzuniyyət haqqı ödənildikdə Transaction yaradır.
/// </summary>
internal sealed class VacationPayPaidDomainEventHandler
    : DomainEventHandler<VacationPayPaidDomainEvent>
{
    private static readonly string[] AzMonthNames =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr"
    ];

    private readonly IVacationPayRepository _vacationPayRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICompanyBalanceService _companyBalanceService;
    private readonly IUnitOfWork _unitOfWork;

    public VacationPayPaidDomainEventHandler(
        IVacationPayRepository vacationPayRepository,
        ITransactionRepository transactionRepository,
        ICompanyBalanceService companyBalanceService,
        IUnitOfWork unitOfWork)
    {
        _vacationPayRepository = vacationPayRepository;
        _transactionRepository = transactionRepository;
        _companyBalanceService = companyBalanceService;
        _unitOfWork = unitOfWork;
    }

    public override async Task Handle(
        VacationPayPaidDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        // 1. Fetch VacationPay
        var vacationPay = await _vacationPayRepository.GetByIdDefaultAsync(
            domainEvent.VacationPayId, cancellationToken);

        if (vacationPay is null)
        {
            throw new InvalidOperationException(
                $"VacationPay {domainEvent.VacationPayId} tapılmadı");
        }

        if (vacationPay.PaidAt is null)
        {
            throw new InvalidOperationException(
                $"VacationPay {domainEvent.VacationPayId} hələ ödənilməyib");
        }

        // 2. Build period label (məzuniyyət dövrü)
        var startMonth = AzMonthNames[vacationPay.VacationStartDate.Month - 1];
        var endMonth = AzMonthNames[vacationPay.VacationEndDate.Month - 1];
        var year = vacationPay.VacationStartDate.Year;

        var periodLabel = vacationPay.VacationStartDate.Month == vacationPay.VacationEndDate.Month
            ? $"{startMonth} {year}"
            : $"{startMonth}-{endMonth} {year}";

        // 3. Create transaction
        var transaction = Transaction.ForVacationPay(
            vacationPay.Id,
            vacationPay.EmployeeFullName,
            vacationPay.TotalAmount,
            domainEvent.PaymentMethod,
            vacationPay.PaidAt.Value,
            periodLabel);

        _transactionRepository.Add(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Recalculate company balance
        await _companyBalanceService.RecalculateAsync(cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
