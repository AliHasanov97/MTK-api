namespace MTK.Modules.Payments.Application.Abstractions;

/// <summary>
/// Payments-in öz abstraksiyası — "cari istifadəçi kimdir" sualına cavab verir,
/// Identity modulunun konkret <c>IUserContext</c>-inə birbaşa bağlanmadan (bu,
/// modullar arası sərhədi pozardı). Konkret tətbiq Infrastructure qatındadır,
/// Identity.Application-a yalnız orada istinad olunur — eynən PaymentsDbContext-in
/// artıq etdiyi kimi (audit actor-u üçün). Presentation (Controller-lər) yalnız
/// bu interfeysi tanıyır.
/// </summary>
public interface ICurrentUserProvider
{
    Guid UserId { get; }
}
