using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.SalaryDeductions;

public sealed class SalaryDeduction : Order
{
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;

    public override Employee? RelatedEmployee => Employee;

    public string District { get; private set; } = string.Empty;
    public int JudgementNo { get; private set; }
    public DateTimeOffset JudgementDate { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public decimal PercentageSalary { get; private set; }
    public decimal StateFee { get; private set; }
    public string Creditor { get; private set; } = string.Empty;
    public decimal Debt { get; private set; }

    private SalaryDeduction() { }

    public static SalaryDeduction Create(
        Guid employeeId,
        string district,
        int judgementNo,
        DateTimeOffset judgementDate,
        DateTimeOffset startDate,
        decimal percentageSalary,
        decimal stateFee,
        string creditor,
        decimal debt,
        Guid createdById)
    {
        var salaryDeduction = new SalaryDeduction
        {
            Id = Guid.NewGuid(),
            Type = OrderType.SalaryDeduction,
            EmployeeId = employeeId,
            District = district,
            JudgementNo = judgementNo,
            JudgementDate = judgementDate,
            StartDate = startDate,
            PercentageSalary = percentageSalary,
            StateFee = stateFee,
            Creditor = creditor,
            Debt = debt,
            CreatedById = createdById
        };

        return salaryDeduction;
    }
}