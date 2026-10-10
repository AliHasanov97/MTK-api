using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Domain.VacationReturnOrders;

/// <summary>
/// Məzuniyyətdən geri qayıtma əmri/sərəncamı
/// VacationReturnApplication-dan convert edildikdə yaradılır
/// </summary>
public sealed class VacationReturnOrder : Order
{
    /// <summary>
    /// Hansı ərizədən yaradılıb
    /// </summary>
    public Guid VacationReturnApplicationId { get; private set; }
    public VacationReturnApplication VacationReturnApplication { get; private set; } = null!;

    /// <summary>
    /// İşçi
    /// </summary>
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    /// <summary>
    /// İşə qayıdış tarixi
    /// </summary>
    public DateTimeOffset ReturnDate { get; private set; }

    /// <summary>
    /// Qeydlər
    /// </summary>
    public string? Notes { get; private set; }

    // Override base property
    public override Employee? RelatedEmployee => Employee;

    private VacationReturnOrder() { }

    /// <summary>
    /// Məzuniyyətdən geri qayıtma əmri yaradır
    /// </summary>
    public static VacationReturnOrder Create(
        Guid vacationReturnApplicationId,
        Guid employeeId,
        DateTimeOffset returnDate,
        Guid createdById,
        string? notes = null)
    {
        var id = Guid.NewGuid();
        var order = new VacationReturnOrder
        {
            Id = id,
            Type = OrderType.VacationReturn,
            VacationReturnApplicationId = vacationReturnApplicationId,
            EmployeeId = employeeId,
            ReturnDate = DateTimeHelper.ToUtcDateOnly(returnDate),
            CreatedById = createdById,
            Notes = notes
        };

        return order;
    }
}