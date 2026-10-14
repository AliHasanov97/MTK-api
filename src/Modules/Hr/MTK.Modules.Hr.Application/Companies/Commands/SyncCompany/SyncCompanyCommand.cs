using MTK.Common.Application.Messaging;

namespace MTK.Modules.Hr.Application.Companies.Commands.SyncCompany;

public sealed record SyncCompanyCommand(Guid CompanyId, string Name, string? Director, string Address) : ICommand;
