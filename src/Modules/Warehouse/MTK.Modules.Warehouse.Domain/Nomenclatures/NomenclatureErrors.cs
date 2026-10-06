using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Warehouse.Domain.Nomenclatures;

public static class NomenclatureErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Nomenclature.NotFound", $"Nomenklatura ID '{id}' tapılmadı");

    public static Error NotFoundByCode(string code) =>
        Error.NotFound("Nomenclature.NotFoundByCode", $"Nomenklatura kodu '{code}' tapılmadı");

    public static Error DuplicateCode(string code) =>
        Error.Conflict("Nomenclature.DuplicateCode", $"Nomenklatura kodu '{code}' artıq mövcuddur");

    public static Error InvalidCode =>
        Error.Validation("Nomenclature.InvalidCode", "Nomenklatura kodu düzgün deyil");

    public static Error InvalidName =>
        Error.Validation("Nomenclature.InvalidName", "Nomenklatura adı düzgün deyil");

    public static Error InvalidMinStockLevel =>
        Error.Validation("Nomenclature.InvalidMinStockLevel", "Minimum ehtiyat səviyyəsi 0-dan kiçik ola bilməz");

    public static Error AlreadyDeleted =>
        Error.Problem("Nomenclature.AlreadyDeleted", "Nomenklatura artıq silinib");

    public static Error CannotDeleteWithStock =>
        Error.Problem("Nomenclature.CannotDeleteWithStock", "Anbarda stoku olan nomenklatura silinə bilməz");
}
