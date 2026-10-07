using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.Nomenclatures;

public static class NomenclatureErrors
{
    public static Error NotFound(Guid id) =>
        new Error("Nomenclature.NotFound", $"Nomenklatura ID '{id}' tapılmadı");

    public static Error NotFoundByCode(string code) =>
        new Error("Nomenclature.NotFoundByCode", $"Nomenklatura kodu '{code}' tapılmadı");

    public static Error DuplicateCode(string code) =>
        new Error("Nomenclature.DuplicateCode", $"Nomenklatura kodu '{code}' artıq mövcuddur");

    public static Error InvalidCode =>
        new Error("Nomenclature.InvalidCode", "Nomenklatura kodu düzgün deyil");

    public static Error InvalidName =>
        new Error("Nomenclature.InvalidName", "Nomenklatura adı düzgün deyil");

    public static Error InvalidMinStockLevel =>
        new Error("Nomenclature.InvalidMinStockLevel", "Minimum ehtiyat səviyyəsi 0-dan kiçik ola bilməz");

    public static Error AlreadyDeleted =>
        new Error("Nomenclature.AlreadyDeleted", "Nomenklatura artıq silinib");

    public static Error CannotDeleteWithStock =>
        new Error("Nomenclature.CannotDeleteWithStock", "Anbarda stoku olan nomenklatura silinə bilməz");
}