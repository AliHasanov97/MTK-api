namespace MTK.Modules.Payments.Domain.Contracts;

/// <summary>
/// Müqavilə üzrə xidmətin hansı dövriliklə hesablandığını müəyyən edir.
/// </summary>
public enum BillingPeriod
{
    Monthly,     // Aylıq (məs. liftə texniki xidmət)
    Quarterly,   // Rüblük
    Yearly,      // İllik
    OneTime      // Birdəfəlik — dövri borc yaratmır
}
