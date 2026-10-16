using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.SalaryDeductions.Events;

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
    public decimal TotalPaid { get; private set; }

    public decimal RemainingDebt => Math.Max(0, Debt - TotalPaid);

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

        salaryDeduction.RaiseDomainEvent(new SalaryDeductionCreatedDomainEvent
        {
            SalaryDeductionId = salaryDeduction.Id,
            EmployeeId = employeeId,
            PercentageSalary = percentageSalary,
            StartDate = startDate,
            Debt = debt
        });

        return salaryDeduction;
    }

    public void RecordPayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Ödəniş məbləği müsbət olmalıdır", nameof(amount));

        if (amount > RemainingDebt)
            throw new InvalidOperationException(
                $"Ödəniş məbləği ({amount}) qalan borcdan ({RemainingDebt}) çoxdur");

        TotalPaid += amount;
        SetUpdatedAt();

        RaiseDomainEvent(new SalaryDeductionPaymentRecordedDomainEvent
        {
            SalaryDeductionId = Id,
            EmployeeId = EmployeeId,
            AmountPaid = amount,
            RemainingDebt = RemainingDebt
        });
    }

    public void Delete()
    {
        SetDeletedAt();
        RaiseDomainEvent(new SalaryDeductionDeletedDomainEvent
        {
            SalaryDeductionId = Id,
            EmployeeId = EmployeeId
        });
    }
}