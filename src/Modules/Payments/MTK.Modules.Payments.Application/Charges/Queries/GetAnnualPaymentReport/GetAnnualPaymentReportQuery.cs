using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Domain.Charges;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetAnnualPaymentReport;

/// <summary>Bütün mənzil/qarajların bir il üzrə aylıq ödəniş qrafiki. <paramref name="PropertyType"/> verilməzsə hər ikisi daxil edilir.</summary>
public sealed record GetAnnualPaymentReportQuery(int Year, PropertyType? PropertyType)
    : IQuery<AnnualPaymentReportResponse>;
