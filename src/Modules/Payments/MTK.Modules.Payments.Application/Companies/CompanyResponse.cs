using MTK.Modules.Payments.Domain.Companies;

namespace MTK.Modules.Payments.Application.Companies;

public sealed record CompanyResponse(
    Guid Id,
    string Name,
    string? Director,
    string Address,
    string Email,
    string? Phone,
    string? WorkHours)
{
    public static CompanyResponse FromDomain(Company company) =>
        new(company.Id, company.Name, company.Director, company.Address, company.Email, company.Phone, company.WorkHours);
}
