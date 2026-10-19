using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Companies.Events;

namespace MTK.Modules.Payments.Domain.Companies;

/// <summary>
/// Şirkətin (MTK-nin) rəsmi profili — ad, direktor, ünvan. CompanyBalance ilə eyni
/// qayda: tək sətirlik aqreqat (bütün sistem üçün bir şirkət). Bu, "mühasibatlıq"
/// tərəfinin (Payments) sahib olduğu həqiqət mənbəyidir — digər modullar (HR və s.)
/// bunu öz snapshot-larında saxlayır (bax: Payments.IntegrationEvents.Companies),
/// əvvəllər hər PDF export-un öz hardcoded ad sətri olması əvəzinə.
/// </summary>
public sealed class Company : Entity
{
    private Company() : base() { }

    public string Name { get; private set; } = string.Empty;
    public string? Director { get; private set; }
    public string Address { get; private set; } = string.Empty;
    public string Email { get; private set; } = "info@mtk.az";
    public string? Phone { get; private set; }
    public string? WorkHours { get; private set; }

    public static Company Create(
        string name,
        string? director,
        string address,
        string email = "info@mtk.az",
        string? phone = null,
        string? workHours = null)
    {
        var company = new Company
        {
            Name = name,
            Director = director,
            Address = address,
            Email = email,
            Phone = phone,
            WorkHours = workHours
        };

        company.SetCreatedAt();
        company.RaiseDomainEvent(new CompanyCreatedDomainEvent
        {
            CompanyId = company.Id,
            Name = name,
            Director = director,
            Address = address
        });

        return company;
    }

    public void Update(
        string name,
        string? director,
        string address,
        string email = "info@mtk.az",
        string? phone = null,
        string? workHours = null)
    {
        Name = name;
        Director = director;
        Address = address;
        Email = email;
        Phone = phone;
        WorkHours = workHours;
        SetUpdatedAt();

        RaiseDomainEvent(new CompanyUpdatedDomainEvent
        {
            CompanyId = Id,
            Name = name,
            Director = director,
            Address = address
        });
    }
}
