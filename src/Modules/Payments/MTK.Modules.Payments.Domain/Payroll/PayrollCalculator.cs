namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Azərbaycan qanunvericiliyinə uyğun əmək haqqı hesablaması — saf, DB-dən asılı
/// olmayan domain xidməti. Dərəcələr (<see cref="PayrollTaxBracket"/>) çağıran
/// tərəfindən (Application layer) artıq müəyyən bir tarix üçün süzülmüş halda verilir;
/// bu sinif yalnız riyazi tətbiqi bilir, "hansı tarixdə hansı dərəcə qüvvədədir"
/// sualı ilə maraqlanmır.
///
/// Qayda: gəlir vergisinin bazası — ümumi əmək haqqından işçinin icbari
/// ödənişləri (DSMF, işsizlik, tibbi sığorta) çıxıldıqdan sonra qalan məbləğdir.
/// İşəgötürənin payı isə işçinin net-ni azaltmır, şirkətin əlavə xərcidir.
/// </summary>
public static class PayrollCalculator
{
    public static PayrollCalculationResult Calculate(
        decimal grossSalary,
        IReadOnlyCollection<PayrollTaxBracket> brackets)
    {
        if (grossSalary < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(grossSalary), "Əmək haqqı mənfi ola bilməz");
        }

        var employeeDsmf = Apply(grossSalary, PayrollRateType.Dsmf, PayrollParty.Employee, brackets);
        var employeeUnemployment = Apply(grossSalary, PayrollRateType.Unemployment, PayrollParty.Employee, brackets);
        var employeeMedicalInsurance = Apply(grossSalary, PayrollRateType.MedicalInsurance, PayrollParty.Employee, brackets);

        var taxableIncome = Math.Max(0m, grossSalary - employeeDsmf - employeeUnemployment - employeeMedicalInsurance);
        var incomeTax = Apply(taxableIncome, PayrollRateType.IncomeTax, PayrollParty.Employee, brackets);

        var netSalary = grossSalary - employeeDsmf - employeeUnemployment - employeeMedicalInsurance - incomeTax;

        var employerDsmf = Apply(grossSalary, PayrollRateType.Dsmf, PayrollParty.Employer, brackets);
        var employerUnemployment = Apply(grossSalary, PayrollRateType.Unemployment, PayrollParty.Employer, brackets);
        var employerMedicalInsurance = Apply(grossSalary, PayrollRateType.MedicalInsurance, PayrollParty.Employer, brackets);

        var totalEmployerCost = grossSalary + employerDsmf + employerUnemployment + employerMedicalInsurance;

        return new PayrollCalculationResult(
            GrossSalary: grossSalary,
            EmployeeDsmf: employeeDsmf,
            EmployeeUnemployment: employeeUnemployment,
            EmployeeMedicalInsurance: employeeMedicalInsurance,
            TaxableIncome: taxableIncome,
            IncomeTax: incomeTax,
            NetSalary: netSalary,
            EmployerDsmf: employerDsmf,
            EmployerUnemployment: employerUnemployment,
            EmployerMedicalInsurance: employerMedicalInsurance,
            TotalEmployerCost: totalEmployerCost);
    }

    /// <summary>
    /// Mütərəqqi (progressive) tətbiq: məbləğin hər pilləyə düşən hissəsinə o
    /// pillənin faizi tətbiq olunur və cəmlənir. Məs. DSMF (işəgötürən): 0-200 → 22%,
    /// 200-8000 → 15%, 8000-∞ → 11%.
    /// </summary>
    private static decimal Apply(
        decimal amount,
        PayrollRateType rateType,
        PayrollParty party,
        IReadOnlyCollection<PayrollTaxBracket> brackets)
    {
        var applicable = brackets
            .Where(b => b.RateType == rateType && b.Party == party)
            .OrderBy(b => b.LowerBound)
            .ToList();

        decimal total = 0m;

        foreach (var bracket in applicable)
        {
            if (amount <= bracket.LowerBound)
            {
                continue;
            }

            var upper = bracket.UpperBound ?? decimal.MaxValue;
            var taxableInBracket = Math.Min(amount, upper) - bracket.LowerBound;

            if (taxableInBracket <= 0)
            {
                continue;
            }

            total += taxableInBracket * bracket.Rate;
        }

        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }
}
