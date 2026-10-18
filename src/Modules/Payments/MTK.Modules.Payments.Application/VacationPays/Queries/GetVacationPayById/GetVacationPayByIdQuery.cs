using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.VacationPays.Queries.GetVacationPayById;

public sealed record GetVacationPayByIdQuery(Guid VacationPayId) : IQuery<VacationPayResponse>;
