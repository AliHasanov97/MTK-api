using Microsoft.Extensions.Configuration;
using MTK.Modules.Hr.Application.Abstractions.Organization;

namespace MTK.Modules.Hr.Infrastructure.Organization;

/// <summary>Appsettings "Hr:Organization" bölməsindən oxunan MTK rekvizitləri.</summary>
internal sealed class OrganizationInfo(IConfiguration configuration) : IOrganizationInfo
{
    public string Name => configuration["Hr:Organization:Name"] ?? "MTK";

    public string? Director => configuration["Hr:Organization:Director"];

    public string Address => configuration["Hr:Organization:Address"] ?? "Bakı şəhəri";
}
