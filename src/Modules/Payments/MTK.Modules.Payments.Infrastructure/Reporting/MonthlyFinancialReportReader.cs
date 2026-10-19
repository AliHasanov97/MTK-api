using Microsoft.EntityFrameworkCore;
using MTK.Modules.Payments.Application.Abstractions.Reporting;
using MTK.Modules.Payments.Application.Reports.GetMonthlyFinancialReport;
using MTK.Modules.Payments.Domain.Charges;
using MTK.Modules.Payments.Domain.Payments;
using MTK.Modules.Payments.Domain.PropertyOwnerships;
using MTK.Modules.Payments.Domain.Rates;
using MTK.Modules.Payments.Domain.SalaryPayments;
using MTK.Modules.Payments.Domain.Transactions;
using MTK.Modules.Payments.Infrastructure.Database;

namespace MTK.Modules.Payments.Infrastructure.Reporting;

/// <summary>
/// "Aylıq hesabat"ın maliyyə hissəsini birbaşa Payments bazasından oxuyur.
///
/// Büdcə düsturları (sistemdə ayrıca büdcə cədvəli yoxdur):
/// <list type="bullet">
/// <item><b>Cari büdcə</b> = seçilmiş ay üzrə borclara tətbiq edilmiş ödənişlər.</item>
/// <item><b>Əvvəlki aylardan qalıq</b> = ay başına qədər bütün gəlir − xərc.</item>
/// <item><b>Ehtiyat büdcə</b> = əvvəlki qalıq + cari ayın xalis nəticəsi, mənfi olarsa 0.</item>
/// <item><b>Avans qalığı</b> = sakinlərin ay sonuna qədər ödəyib hələ borclara tətbiq etmədiyi məbləğ.</item>
/// <item><b>Ümumi büdcə</b> = cari büdcə + ehtiyat büdcə + avans qalığı.</item>
/// </list>
/// Bu, nümunə sənəddəki "ümumi büdcə = cari + ehtiyat + avans" məntiqini təkrarlayır.
/// </summary>
internal sealed class MonthlyFinancialReportReader : IMonthlyFinancialReportReader
{
    private const decimal DebtorThreshold = 2.00m;

    private static readonly string[] AzMonths =
    [
        "Yanvar", "Fevral", "Mart", "Aprel", "May", "İyun",
        "İyul", "Avqust", "Sentyabr", "Oktyabr", "Noyabr", "Dekabr",
    ];

    private readonly PaymentsDbContext _db;

    public MonthlyFinancialReportReader(PaymentsDbContext db)
    {
        _db = db;
    }

    public async Task<MonthlyFinancialReportResponse> ReadAsync(
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var start = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMonths(1);
        var asOf = end.AddTicks(-1);
        var period = $"{year:D4}-{month:D2}";

        var company = await _db.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        // Cari ay üçün yaradılmış sakin borcları — ləğv edilmişlər hesaba düşmür.
        var periodCharges = await _db.Charges
            .AsNoTracking()
            .Include(c => c.Owner)
            .Include(c => c.Apartment).ThenInclude(a => a!.Building)
            .Include(c => c.Garage)
            .Where(c => c.OwnerId != null && c.Period == period && c.IssuedOn < end && c.Status != ChargeStatus.Cancelled)
            .ToListAsync(cancellationToken);

        var monthCharges = periodCharges.Sum(c => c.Amount);
        // PaidAmount is cumulative and can include allocations made after the report month.
        // Reconstruct the month-end balance from allocations recorded before the next month.
        var periodChargeIds = periodCharges.Select(c => c.Id).ToList();
        var periodAllocations = await _db.PaymentAllocations
            .AsNoTracking()
            .Where(a => periodChargeIds.Contains(a.ChargeId) && a.CreatedAt < end)
            .Select(a => new { a.ChargeId, a.Amount, a.CreatedAt })
            .ToListAsync(cancellationToken);
        var allocatedByPeriodCharge = periodAllocations
            .GroupBy(a => a.ChargeId)
            .ToDictionary(g => g.Key, g => g.Sum(a => a.Amount));
        var monthPaid = periodAllocations.Sum(a => a.Amount);
        var monthDebt = Math.Max(0, monthCharges - monthPaid);

        // MTK-nın ümumi borc vəziyyəti seçilmiş ayın sonuna olan yığılan qalıqdır.
        var allOwnerCharges = await _db.Charges
            .AsNoTracking()
            .Where(c => c.OwnerId != null && c.Status != ChargeStatus.Cancelled && c.IssuedOn < end)
            .Select(c => new { c.Id, c.ApartmentId, c.GarageId, c.Amount })
            .ToListAsync(cancellationToken);
        var allOwnerChargeIds = allOwnerCharges.Select(c => c.Id).ToList();
        var allocationsByEnd = await _db.PaymentAllocations
            .AsNoTracking()
            .Where(a => allOwnerChargeIds.Contains(a.ChargeId) && a.CreatedAt < end)
            .GroupBy(a => a.ChargeId)
            .Select(g => new { ChargeId = g.Key, Amount = g.Sum(a => a.Amount) })
            .ToDictionaryAsync(x => x.ChargeId, x => x.Amount, cancellationToken);
        decimal RemainingAtEnd(Guid chargeId, decimal amount) =>
            Math.Max(0m, amount - allocationsByEnd.GetValueOrDefault(chargeId));
        var totalApartmentDebt = allOwnerCharges
            .Where(c => c.ApartmentId != null)
            .Sum(c => RemainingAtEnd(c.Id, c.Amount));
        var totalGarageDebt = allOwnerCharges
            .Where(c => c.GarageId != null)
            .Sum(c => RemainingAtEnd(c.Id, c.Amount));
        var totalOutstandingDebt = allOwnerCharges
            .Sum(c => RemainingAtEnd(c.Id, c.Amount));

        // Gəlir/xərc ledger-i. "Əvvəlki aylardan qalıq" ay başına qədər yığımdır.
        var incomeBefore = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.Direction == TransactionDirection.Income && t.TransactionDate < start)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        var expenseBefore = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.Direction == TransactionDirection.Expense && t.TransactionDate < start)
            .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

        var monthTransactions = await _db.Transactions
            .AsNoTracking()
            .Where(t => t.TransactionDate >= start && t.TransactionDate < end)
            .OrderBy(t => t.TransactionDate)
            .ToListAsync(cancellationToken);

        var monthIncome = monthTransactions
            .Where(t => t.Direction == TransactionDirection.Income)
            .Sum(t => t.Amount);

        var monthExpenses = monthTransactions
            .Where(t => t.Direction == TransactionDirection.Expense)
            .Sum(t => t.Amount);

        var carryOver = incomeBefore - expenseBefore;
        var monthNet = monthIncome - monthExpenses;
        var reserveBudget = Math.Max(0, carryOver + monthNet);

        // Avans qalığı yığılan saldo olduğundan hesabat ayının sonuna qədərki bütün sakin
        // ödənişlərini götürürük və həmin tarixədək borclara paylanmış hissəni çıxırıq.
        var monthPaymentIds = await _db.Payments
            .AsNoTracking()
            .Where(p => p.OwnerId != null && p.Status == PaymentStatus.Completed && p.PaymentDate < end)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);
        var allocatedByMonthPayment = await _db.PaymentAllocations
            .AsNoTracking()
            .Where(a => monthPaymentIds.Contains(a.PaymentId) && a.CreatedAt < end)
            .GroupBy(a => a.PaymentId)
            .Select(g => new { PaymentId = g.Key, Amount = g.Sum(a => a.Amount) })
            .ToDictionaryAsync(x => x.PaymentId, x => x.Amount, cancellationToken);
        var advancesByOwner = await _db.Payments
            .AsNoTracking()
            .Where(p => p.OwnerId != null && p.Status == PaymentStatus.Completed && p.PaymentDate < end)
            .Select(p => new { OwnerId = p.OwnerId!.Value, p.Id, p.Amount })
            .ToListAsync(cancellationToken);
        var advanceByOwner = advancesByOwner
            .GroupBy(p => p.OwnerId)
            .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount - allocatedByMonthPayment.GetValueOrDefault(p.Id)));
        var positiveAdvanceOwnerIds = advanceByOwner.Keys
            .Where(id => advanceByOwner.GetValueOrDefault(id) > 0.004m)
            .ToHashSet();

        var advanceBalance = positiveAdvanceOwnerIds
            .Sum(id => advanceByOwner.GetValueOrDefault(id));

        var advanceOwners = await _db.Owners
            .AsNoTracking()
            .Where(owner => positiveAdvanceOwnerIds.Contains(owner.Id))
            .ToDictionaryAsync(owner => owner.Id, owner => owner.FullName, cancellationToken);

        var ownerships = await _db.PropertyOwnerships
            .AsNoTracking()
            .Include(o => o.Owner)
            .Include(o => o.Apartment).ThenInclude(a => a!.Building)
            .Include(o => o.Garage)
            .ToListAsync(cancellationToken);

        var ownershipsByOwner = ownerships
            .GroupBy(o => o.OwnerId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var advances = positiveAdvanceOwnerIds
            .Select(ownerId =>
            {
                var ownerOwnerships = ownershipsByOwner.GetValueOrDefault(ownerId) ?? [];
                var ownerName = advanceOwners.GetValueOrDefault(ownerId)
                    ?? ownerOwnerships.FirstOrDefault()?.Owner?.FullName
                    ?? "—";
                var properties = ownerOwnerships.Count == 0
                    ? "—"
                    : string.Join(", ", ownerOwnerships.Select(PropertyLabel).Distinct());
                var amount = advanceByOwner.GetValueOrDefault(ownerId);
                return new AdvanceRow(ownerName, properties, amount);
            })
            .OrderByDescending(a => a.Amount)
            .ThenBy(a => a.OwnerName)
            .ToList();

        var monthPaymentCount = await _db.Payments
            .AsNoTracking()
            .CountAsync(p =>
                p.Status == PaymentStatus.Completed &&
                p.PaymentDate >= start && p.PaymentDate < end &&
                (p.OwnerId != null || p.CustomerId != null), cancellationToken);

        var salaries = await _db.SalaryPayments
            .AsNoTracking()
            .Where(s => s.Status == SalaryPaymentStatus.Paid &&
                        s.PaymentDate >= start &&
                        s.PaymentDate < end)
            .OrderBy(s => s.EmployeeFullName)
            .ToListAsync(cancellationToken);

        // Tariflər — ayın sonunda qüvvədə olan tarif.
        var apartmentRate = await _db.Rates
            .AsNoTracking()
            .Where(r => r.RateType == RateType.PerSquareMeter &&
                        r.EffectiveFrom <= asOf &&
                        (r.EffectiveTo == null || r.EffectiveTo > asOf))
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken);

        var activeGarageRates = await _db.Rates
            .AsNoTracking()
            .Where(r => r.RateType == RateType.FixedGarage &&
                        r.EffectiveFrom <= asOf &&
                        (r.EffectiveTo == null || r.EffectiveTo > asOf))
            .OrderByDescending(r => r.EffectiveFrom)
            .ToListAsync(cancellationToken);

        var garageRates = activeGarageRates
            .GroupBy(r => r.GarageType)
            .Select(g => g.First())
            .OrderBy(r => r.GarageType is null ? int.MaxValue : (int)r.GarageType.Value)
            .Select(r => new GarageRateRow(GarageTypeLabel(r.GarageType), r.Amount))
            .ToList();

        var periodApartmentCharges = periodCharges.Where(c => c.ApartmentId != null).ToList();
        var periodGarageCharges = periodCharges.Where(c => c.GarageId != null).ToList();

        var registeredApartmentIds = ownerships
            .Where(o => o.ApartmentId != null)
            .Select(o => o.ApartmentId!.Value)
            .Distinct()
            .ToList();

        var registeredGarageIds = ownerships
            .Where(o => o.GarageId != null)
            .Select(o => o.GarageId!.Value)
            .Distinct()
            .ToList();

        var totalApartmentArea = ownerships
            .Where(o => o.ApartmentId != null && o.Apartment is not null)
            .GroupBy(o => o.ApartmentId!.Value)
            .Sum(g => g.First().Apartment!.AreaSquareMeters);

        var unpaidApartmentCount = periodApartmentCharges
            .Where(c => c.Amount - allocatedByPeriodCharge.GetValueOrDefault(c.Id) > 0.004m)
            .Select(c => c.ApartmentId!.Value)
            .Distinct()
            .Count();

        var unpaidGarageCount = periodGarageCharges
            .Where(c => c.Amount - allocatedByPeriodCharge.GetValueOrDefault(c.Id) > 0.004m)
            .Select(c => c.GarageId!.Value)
            .Distinct()
            .Count();

        var apartments = new ApartmentReportSummary(
            registeredApartmentIds.Count,
            unpaidApartmentCount,
            totalApartmentArea,
            apartmentRate?.Amount,
            periodApartmentCharges.Sum(c => c.Amount),
            totalApartmentDebt);

        var garages = new GarageReportSummary(
            registeredGarageIds.Count,
            unpaidGarageCount,
            periodGarageCharges.Sum(c => c.Amount),
            totalGarageDebt,
            garageRates);

        // Cari ay üzrə borclular + onların bütün açıq dövrləri.
        var debtorCurrentCharges = periodCharges
            .Where(c => c.Amount - allocatedByPeriodCharge.GetValueOrDefault(c.Id) > DebtorThreshold)
            .ToList();

        var debtors = debtorCurrentCharges
            .GroupBy(c => c.ApartmentId ?? c.GarageId!.Value)
            .Select(g =>
            {
                var charge = g.First();
                var isApartment = charge.ApartmentId.HasValue;
                var totalDebt = g.Sum(c => c.Amount - allocatedByPeriodCharge.GetValueOrDefault(c.Id));

                return new DebtorRow(
                    isApartment ? "Mənzil" : "Qaraj",
                    isApartment ? charge.Apartment?.ApartmentNumber ?? "—" : charge.Garage?.GarageNumber ?? "—",
                    isApartment ? charge.Apartment?.Building?.Name : null,
                    charge.Owner?.FullName ?? "—",
                    charge.AreaSquareMeters,
                    g.Sum(c => c.Amount),
                    g.Sum(c => allocatedByPeriodCharge.GetValueOrDefault(c.Id)),
                    AzMonths[month - 1] + " " + year,
                    totalDebt);
            })
            .OrderByDescending(d => d.TotalDebt)
            .ThenBy(d => d.Number)
            .ToList();

        var expenseBreakdown = monthTransactions
            .Where(t => t.Direction == TransactionDirection.Expense)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Digər" : t.Category)
            .Select(g => new ExpenseCategoryRow(g.Key, g.Count(), g.Sum(t => t.Amount)))
            .OrderByDescending(x => x.Amount)
            .ToList();

        var expenseDetails = monthTransactions
            .Where(t => t.Direction == TransactionDirection.Expense)
            .Select(t => new ExpenseDetailRow(t.TransactionDate, t.Category, t.Description, t.Amount))
            .ToList();

        var payroll = salaries
            .Select(s => new PayrollPaymentRow(
                s.EmployeeFullName,
                s.SalaryYear,
                s.SalaryMonth,
                s.GrossSalary,
                PaymentMethodLabel(s.PaymentMethod),
                s.PaymentDate))
            .ToList();

        return new MonthlyFinancialReportResponse(
            year,
            month,
            company?.Name ?? "MTK",
            company?.Address,
            company?.Director,
            CurrentBudget: monthPaid,
            ReserveBudget: reserveBudget,
            TotalBudget: monthPaid + reserveBudget + advanceBalance,
            AdvanceBalance: advanceBalance,
            CarryOver: carryOver,
            MonthIncome: monthIncome,
            MonthExpenses: monthExpenses,
            MonthNet: monthNet,
            MonthCharges: monthCharges,
            MonthPaid: monthPaid,
            MonthDebt: monthDebt,
            TotalOutstandingDebt: totalOutstandingDebt,
            PaymentCount: monthPaymentCount,
            ExpenseCount: expenseDetails.Count,
            AdvanceOwnerCount: positiveAdvanceOwnerIds.Count,
            Apartments: apartments,
            Garages: garages,
            ExpenseBreakdown: expenseBreakdown,
            Advances: advances,
            Debtors: debtors,
            ExpenseDetails: expenseDetails,
            PayrollPayments: payroll);
    }

    private static string PropertyLabel(PropertyOwnership ownership)
    {
        if (ownership.Apartment is { } apartment)
        {
            var building = apartment.Building?.Name;
            return string.IsNullOrWhiteSpace(building)
                ? $"Mənzil {apartment.ApartmentNumber}"
                : $"{building}, mənzil {apartment.ApartmentNumber}";
        }

        return ownership.Garage is { } garage
            ? $"Qaraj {garage.GarageNumber}"
            : "—";
    }

    private static string GarageTypeLabel(GarageType? type) => type switch
    {
        GarageType.OpenParking => "Açıq parkinq",
        GarageType.CoveredGarage => "Örtülü qaraj",
        GarageType.Storage => "Anbar",
        _ => "Ümumi qaraj tarifi",
    };

    private static string PaymentMethodLabel(PaymentMethod method) => method switch
    {
        PaymentMethod.Cash => "Nağd",
        PaymentMethod.BankTransfer => "Bank köçürməsi",
        PaymentMethod.Card => "Kart",
        _ => method.ToString(),
    };
}
