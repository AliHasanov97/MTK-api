using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Employees;

/// <summary>
/// HR-dəki Employee-in bu moduldakı adı/əmək haqqı/status snapshot-u (Id = HR Employee.Id).
/// Yalnız "kim, nə qədər əmək haqqı alır və aktivdirmi?" sualına modullar arası DB join
/// olmadan cavab vermək üçün (əmək haqqı hesablanması, PayrollRun) — HR-in
/// EmployeeCreated/EmployeeUpdated integration event-ləri ilə sinxronlaşır.
/// </summary>
public sealed class Employee : Entity
{
    private Employee(Guid id, string fullName, decimal baseSalary, bool isActive) : base(id)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
        IsActive = isActive;
    }

    private Employee() : base() { }

    public string FullName { get; private set; } = string.Empty;
    public decimal BaseSalary { get; private set; }

    /// <summary>İşçi aktivdirmi? İşdən çıxmış (Inactive) işçilərə əmək haqqı hesablanmır.</summary>
    public bool IsActive { get; private set; }

    public static Employee Create(Guid id, string fullName, decimal baseSalary, bool isActive = true)
    {
        var employee = new Employee(id, fullName, baseSalary, isActive);
        employee.SetCreatedAt();
        return employee;
    }

    public void Update(string fullName, decimal baseSalary, bool isActive)
    {
        FullName = fullName;
        BaseSalary = baseSalary;
        IsActive = isActive;
        SetUpdatedAt();
    }
}
