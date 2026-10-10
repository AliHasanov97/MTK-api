using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.SalaryDeductions.DeleteSalaryDeduction;

public sealed class DeleteSalaryDeductionCommand : ICommand
{
    public Guid Id { get; set; }
}
