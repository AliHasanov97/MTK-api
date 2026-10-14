using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Companies;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Companies.Commands.UpsertCompany;

internal sealed class UpsertCompanyCommandHandler : ICommandHandler<UpsertCompanyCommand, CompanyResponse>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompanyResponse>> Handle(UpsertCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetAsync(cancellationToken);

        if (company is null)
        {
            company = Company.Create(request.Name, request.Director, request.Address);
            _companyRepository.Add(company);
        }
        else
        {
            company.Update(request.Name, request.Director, request.Address);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CompanyResponse.FromDomain(company));
    }
}
