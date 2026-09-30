using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Contracts.Queries.GetContractById;

public sealed record GetContractByIdQuery(Guid ContractId) : IQuery<ContractResponse>;
