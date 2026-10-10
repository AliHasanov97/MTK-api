using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.SalaryDeductions.AddSalaryDeduction;

public sealed class AddSalaryDeductionCommand : ICommand<AddSalaryDeductionResponse>
{
    public Guid EmployeeId { get; set; }
    public string District { get; set; } = string.Empty;
    public int JudgementNo { get; set; }
    public DateTimeOffset JudgementDate { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public int PercentageSalary { get; set; }
    public decimal StateFee { get; set; }
    public string Creditor { get; set; } = string.Empty;
    public decimal Debt { get; set; }
}
