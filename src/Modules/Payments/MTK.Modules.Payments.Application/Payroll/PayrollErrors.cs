using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Application.Payroll;

public static class PayrollErrors
{
    public static Error RunNotFound(Guid id) => new(
        "PayrollRun.NotFound",
        $"PayrollRun tapılmadı: {id}");

    public static readonly Error NotApproved = new(
        "PayrollRun.NotApproved",
        "Ödəniş yalnız təsdiqlənmiş (Approved) hesablama üçün edilə bilər — əvvəlcə təsdiqləyin");

    public static readonly Error NotCalculated = new(
        "PayrollRun.NotCalculated",
        "Yalnız hesablanmış (Calculated) run təsdiqlənə bilər");

    public static Error EmployeeNotInRun(Guid employeeId) => new(
        "PayrollRun.EmployeeNotInRun",
        $"Bu dövrdə {employeeId} işçisi üçün qəbz tapılmadı");

    public static readonly Error AlreadyPaid = new(
        "PayrollRun.AlreadyPaid",
        "Bu qəbz artıq ödənilib");

    public static readonly Error NothingToPay = new(
        "PayrollRun.NothingToPay",
        "Bu run üzrə bütün qəbzlər artıq ödənilib");
}
