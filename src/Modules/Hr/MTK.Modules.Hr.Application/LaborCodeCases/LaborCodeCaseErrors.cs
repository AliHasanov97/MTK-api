using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.LaborCodeCases;

public static class LaborCodeCaseErrors
{
    public static Error NotFound => new("LaborCodeCase.NotFound", "Əmək Məcəlləsi maddəsi tapılmadı.");
}
