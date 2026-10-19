using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Companies;

namespace MTK.Modules.Payments.Application.Companies.Commands.UpsertCompany;

/// <summary>Tək sətir — yoxdursa yaradır, varsa yeniləyir (Company CompanyBalance ilə eyni qayda: tək aqreqat).</summary>
public sealed record UpsertCompanyCommand(
    string Name,
    string? Director,
    string Address,
    string Email = "info@mtk.az",
    string? Phone = null,
    string? WorkHours = null) : ICommand<CompanyResponse>;
