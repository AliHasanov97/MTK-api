using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Employees;

/// <summary>
/// HR-dəki Employee-in bu moduldakı adı/əmək haqqı snapshot-u (Id = HR Employee.Id).
/// Yalnız "kim, nə qədər əmək haqqı alır?" sualına modullar arası DB join olmadan
/// cavab vermək üçün (əmək haqqı hesablanması, PayrollRun) — HR-in
/// EmployeeCreated/EmployeeUpdated integration event-ləri ilə sinxronlaşır.
/// </summary>
public sealed class Employee : Entity
{
    private Employee(Guid id, string fullName, decimal baseSalary) : base(id)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
    }

    private Employee() : base() { }

    public string FullName { get; private set; } = string.Empty;
    public decimal BaseSalary { get; private set; }

    public static Employee Create(Guid id, string fullName, decimal baseSalary)
    {
        var employee = new Employee(id, fullName, baseSalary);
        employee.SetCreatedAt();
        return employee;
    }

    public void Update(string fullName, decimal baseSalary)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
        SetUpdatedAt();
    }
}
