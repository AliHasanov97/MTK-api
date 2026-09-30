namespace MTK.Modules.Payments.Domain.VendorCharges;

public enum VendorChargeStatus
{
    Unpaid,        // Ödənilməyib
    PartiallyPaid, // Qismən ödənilib
    Paid,          // Ödənilib
    Cancelled      // Ləğv edilib (səhv yaranmış borc)
}
