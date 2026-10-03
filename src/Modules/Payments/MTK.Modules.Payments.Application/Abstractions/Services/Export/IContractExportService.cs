using MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;

namespace MTK.Modules.Payments.Application.Abstractions.Services.Export;

public interface IContractExportService
{
    MemoryStream ExportToPdf(ContractResponse contract);
}
