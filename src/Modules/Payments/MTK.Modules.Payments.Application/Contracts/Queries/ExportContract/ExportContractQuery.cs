using MTK.Common.Application.Exporting;
using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Queries.ExportContract;

public sealed record ExportContractQuery(Guid ContractId) : IQuery<ExportFileResult>;
