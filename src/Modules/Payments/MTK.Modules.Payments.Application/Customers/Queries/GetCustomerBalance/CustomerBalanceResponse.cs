namespace MTK.Modules.Payments.Application.Customers.Queries.GetCustomerBalance;

/// <summary>
/// Müştərinin hesab vəziyyəti.
/// <para><b>OutstandingDebt</b> — açıq borcların qalığı (müştəri bizə borcludur).</para>
/// <para><b>Advance</b> — borca bölünməmiş, "xərclənməmiş" ödəniş qalığı; növbəti borc yaranan kimi ona avtomatik tətbiq olunur.</para>
/// </summary>
public sealed record CustomerBalanceResponse(
    Guid CustomerId,
    decimal OutstandingDebt,
    decimal Advance,
    decimal TotalPaid);
