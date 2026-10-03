using AutoMapper;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.CompanyBalances.Queries.GetCompanyBalance;

internal sealed class GetCompanyBalanceQueryHandler : IQueryHandler<GetCompanyBalanceQuery, CompanyBalanceResponse>
{
    private readonly ICompanyBalanceRepository _companyBalanceRepository;
    private readonly IMapper _mapper;

    public GetCompanyBalanceQueryHandler(ICompanyBalanceRepository companyBalanceRepository, IMapper mapper)
    {
        _companyBalanceRepository = companyBalanceRepository;
        _mapper = mapper;
    }

    public async Task<Result<CompanyBalanceResponse>> Handle(
        GetCompanyBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var balance = await _companyBalanceRepository.GetAsync(cancellationToken);

        // Not an error — it just means no transaction has ever been posted yet,
        // so the balance is genuinely zero (nothing to recalculate from).
        if (balance is null)
        {
            return Result.Success(new CompanyBalanceResponse(0, 0, 0));
        }

        return Result.Success(_mapper.Map<CompanyBalanceResponse>(balance));
    }
}
