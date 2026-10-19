namespace MTK.Modules.Buildings.Domain.Appeals;

/// <summary>
/// Müraciət kateqoriyası.
/// </summary>
public enum AppealCategory
{
    /// <summary>Təmir-bərpa işləri (ən çox istifadə olunan).</summary>
    Maintenance = 1,

    /// <summary>Texniki problemlər (su, işıq, qaz, istilik).</summary>
    Technical = 2,

    /// <summary>Maliyyə məsələləri (ödəniş, borc, hesablaşma).</summary>
    Financial = 3,

    /// <summary>Təmizlik məsələləri (ümumi sahələr, zibil).</summary>
    Cleanliness = 4,

    /// <summary>Təhlükəsizlik və mühafizə.</summary>
    Security = 5,

    /// <summary>Dayanacaq və nəqliyyat.</summary>
    Parking = 6,

    /// <summary>Səs-küy şikayəti.</summary>
    Noise = 7,

    /// <summary>Ümumi müraciət (digər).</summary>
    General = 8
}
