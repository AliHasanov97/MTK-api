using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.PayrollTaxBrackets;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Queries.GetPayrollTaxBrackets;

internal sealed class GetPayrollTaxBracketsQueryHandler
    : IQueryHandler<GetPayrollTaxBracketsQuery, IReadOnlyCollection<PayrollTaxBracketResponse>>
{
    private readonly IPayrollTaxBracketRepository _payrollTaxBracketRepository;

    public GetPayrollTaxBracketsQueryHandler(IPayrollTaxBracketRepository payrollTaxBracketRepository)
    {
        _payrollTaxBracketRepository = payrollTaxBracketRepository;
    }

    public async Task<Result<IReadOnlyCollection<PayrollTaxBracketResponse>>> Handle(
        GetPayrollTaxBracketsQuery request,
        CancellationToken cancellationToken)
    {
        var brackets = await _payrollTaxBracketRepository.ListAsync(cancellationToken);

        var response = brackets
            .OrderBy(b => b.RateType)
            .ThenBy(b => b.Party)
            .ThenBy(b => b.LowerBound)
            .ThenByDescending(b => b.EffectiveFrom)
            .Select(PayrollTaxBracketResponse.FromDomain)
            .ToList();

        return Result.Success<IReadOnlyCollection<PayrollTaxBracketResponse>>(response);
    }
}
