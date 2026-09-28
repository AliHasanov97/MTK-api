using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Rates.Queries.GetCurrentRates;

public sealed record GetCurrentRatesQuery : IQuery<IReadOnlyCollection<RateResponse>>;
