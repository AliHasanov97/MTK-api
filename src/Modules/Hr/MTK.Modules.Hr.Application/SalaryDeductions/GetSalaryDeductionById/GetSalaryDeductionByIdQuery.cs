using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.SalaryDeductions.GetSalaryDeductionById;

public sealed class GetSalaryDeductionByIdQuery : IQuery<GetSalaryDeductionByIdResponse>
{
    public Guid Id { get; set; }
}
