using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Charges.Commands.GenerateChargesForPeriod;

/// <summary>
/// Verilmiş dövr ("yyyy-MM") üçün sakin və tədarükçü borclarını əl ilə yaradır (buraxılmış ayı bərpa etmək üçün).
/// Hələ borcu olmayan mənzil/qaraj/xidmətlər üçün yaradır. Planlı işdən fərqli olaraq ləğv edilmiş borclar
/// "yoxdur" sayılır və həmin əmlak üçün yenidən yaradılır.
/// </summary>
public sealed record GenerateChargesForPeriodCommand(string Period) : ICommand<GenerateChargesForPeriodResponse>;

public sealed record GenerateChargesForPeriodResponse(
    string Period,
    int ResidentChargesCreated,
    int ApartmentsSkippedNoRate,
    int GaragesSkippedNoRate,
    int VendorChargesCreated);
