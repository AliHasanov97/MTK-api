using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Companies;

namespace MTK.Modules.Hr.Application.Companies.Commands.SyncCompany;

internal sealed class SyncCompanyCommandHandler : ICommandHandler<SyncCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SyncCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SyncCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdDefaultAsync(request.CompanyId, cancellationToken);
        if (company is not null)
        {
            company.Update(request.Name, request.Director, request.Address);
        }
        else
        {
            company = Company.Create(request.CompanyId, request.Name, request.Director, request.Address);
            _companyRepository.Add(company);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
