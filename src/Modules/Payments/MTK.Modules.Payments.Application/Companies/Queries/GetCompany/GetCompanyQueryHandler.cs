using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Companies.Queries.GetCompany;

internal sealed class GetCompanyQueryHandler : IQueryHandler<GetCompanyQuery, CompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyResponse>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetAsync(cancellationToken);
        if (company is null)
        {
            return Result.Failure<CompanyResponse>(new Error(
                "Company.NotFound",
                "Şirkət profili hələ qurulmayıb"));
        }

        return Result.Success(CompanyResponse.FromDomain(company));
    }
}
