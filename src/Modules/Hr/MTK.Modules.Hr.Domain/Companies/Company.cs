using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Domain.Companies;

/// <summary>
/// Payments-dəki Company-nin bu moduldakı snapshot-u (Id = Payments Company.Id).
/// Şirkət profilinin həqiqət mənbəyi "mühasibatlıq" tərəfidir (Payments) — burada
/// yalnız PDF-lərdə (bax: OrganizationInfo) göstərmək üçün güzgü saxlanılır,
/// Payments-in CompanyCreated/CompanyUpdated integration event-ləri ilə sinxronlaşır.
/// </summary>
public sealed class Company : Entity
{
    private Company(Guid id, string name, string? director, string address) : base(id)
    {
        Name = name;
        Director = director;
        Address = address;
    }

    private Company() : base() { }

    public string Name { get; private set; } = string.Empty;
    public string? Director { get; private set; }
    public string Address { get; private set; } = string.Empty;

    public static Company Create(Guid id, string name, string? director, string address)
    {
        var company = new Company(id, name, director, address);
        company.SetCreatedAt();
        return company;
    }

    public void Update(string name, string? director, string address)
    {
        Name = name;
        Director = director;
        Address = address;
        SetUpdatedAt();
    }
}
