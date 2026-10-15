using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.PayrollTaxBrackets;

namespace MTK.Modules.Payments.Application.PayrollTaxBrackets.Queries.GetPayrollTaxBrackets;

/// <summary>Bütün dərəcə pillələri (tarixçə və gələcək daxil) — UI-da "Tariflər" səhifəsinin eyni fəlsəfəsi.</summary>
public sealed record GetPayrollTaxBracketsQuery : IQuery<IReadOnlyCollection<PayrollTaxBracketResponse>>;
