namespace MTK.Modules.Payments.Domain.VendorCharges;

/// <summary>Borcun haradan yarandığı — hesabat və audit üçün.</summary>
public enum VendorChargeSource
{
    ServiceSchedule,  // Müqavilə cədvəli üzrə avtomatik (aylıq/rüblük/illik)
    GoodsDelivery,    // Mal tədarükü (qaimə) üzrə
    Manual            // Əl ilə yaradılmış düzəliş
}
