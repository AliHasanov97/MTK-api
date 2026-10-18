using MTK.Modules.Payments.Domain.Repositories;
using PayslipWithPeriodDto = MTK.Modules.Payments.Domain.Repositories.PayslipWithPeriodDto;

namespace MTK.Modules.Payments.Application.Services.AverageWage;

/// <summary>
/// Azərbaycan Əmək Məcəlləsinə uyğun orta əmək haqqı hesablaması.
/// </summary>
internal sealed class AverageWageCalculator : IAverageWageCalculator
{
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;
    private readonly IPayrollRunRepository _payrollRunRepository;

    /// <summary>
    /// Orta günlük əmək haqqı hesablamasında istifadə olunan sabit (Əmək Məcəlləsinə görə).
    /// 365 gün ÷ 12 ay = 30.416666... ≈ 30.4
    /// </summary>
    private const decimal AverageDaysPerMonth = 30.4m;

    public AverageWageCalculator(
        ISalaryPaymentRepository salaryPaymentRepository,
        IPayrollRunRepository payrollRunRepository)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
        _payrollRunRepository = payrollRunRepository;
    }

    public async Task<AverageWageResult> CalculateAsync(
        Guid employeeId,
        DateTime referenceDate,
        CancellationToken cancellationToken = default)
    {
        // 1. Referans tarixindən əvvəlki 12 ayın başlanğıc və son tarixlərini hesabla
        var toDate = new DateTime(referenceDate.Year, referenceDate.Month, 1).AddDays(-1);
        var fromDate = toDate.AddMonths(-11);

        var fromYear = fromDate.Year;
        var fromMonth = fromDate.Month;
        var toYear = toDate.Year;
        var toMonth = toDate.Month;

        // 2. Hər iki sistemdən (SalaryPayment və Payslip) məlumatları götür
        var salaryPayments = await _salaryPaymentRepository.GetByEmployeeAndPeriodRangeAsync(
            employeeId, fromYear, fromMonth, toYear, toMonth, cancellationToken);

        var payslips = await _payrollRunRepository.GetPayslipsByEmployeeAndPeriodRangeAsync(
            employeeId, fromYear, fromMonth, toYear, toMonth, cancellationToken);

        // 3. Hər ay üçün məlumatları birləşdir (Payslip prioritet)
        var monthlyWages = new Dictionary<(int Year, int Month), MonthlyWageBreakdown>();

        // 3a. Payslip-ləri əlavə et (avtomatik sistem, prioritet)
        foreach (var payslip in payslips)
        {
            var key = (payslip.Year, payslip.Month);
            monthlyWages[key] = new MonthlyWageBreakdown(
                Year: payslip.Year,
                Month: payslip.Month,
                BaseSalary: payslip.BaseSalary,
                TotalBonuses: payslip.BonusAmount,
                TotalAdjustments: payslip.AdjustmentAmount,
                GrossWage: payslip.GrossSalary);
        }

        // 3b. SalaryPayment-ləri əlavə et (yalnız Payslip olmayan aylar üçün)
        foreach (var payment in salaryPayments)
        {
            var key = (payment.SalaryYear, payment.SalaryMonth);
            if (!monthlyWages.ContainsKey(key))
            {
                monthlyWages[key] = new MonthlyWageBreakdown(
                    Year: payment.SalaryYear,
                    Month: payment.SalaryMonth,
                    BaseSalary: payment.BaseSalary,
                    TotalBonuses: payment.TotalBonuses,
                    TotalAdjustments: payment.TotalAdjustments,
                    GrossWage: payment.GrossSalary);
            }
        }

        // 4. Əgər heç bir məlumat yoxdursa, xəta
        if (monthlyWages.Count == 0)
        {
            throw new InvalidOperationException(
                $"İşçinin ({employeeId}) son 12 ay ({fromDate:yyyy-MM} - {toDate:yyyy-MM}) üçün heç bir maaş məlumatı tapılmadı");
        }

        // 5. Orta aylıq əmək haqqını hesabla
        var totalGrossWage = monthlyWages.Values.Sum(w => w.GrossWage);
        var monthsIncluded = monthlyWages.Count;
        var averageMonthlyWage = totalGrossWage / monthsIncluded;

        // 6. Orta günlük əmək haqqını hesabla (Əmək Məcəlləsinə görə ÷ 30.4)
        var averageDailyWage = averageMonthlyWage / AverageDaysPerMonth;

        // 7. Breakdown-u tarixə görə sırala
        var breakdown = monthlyWages.Values
            .OrderBy(w => w.Year)
            .ThenBy(w => w.Month)
            .ToList();

        return new AverageWageResult(
            AverageMonthlyWage: Math.Round(averageMonthlyWage, 2),
            AverageDailyWage: Math.Round(averageDailyWage, 2),
            MonthsIncluded: monthsIncluded,
            FromDate: fromDate,
            ToDate: toDate,
            Breakdown: breakdown);
    }
}
