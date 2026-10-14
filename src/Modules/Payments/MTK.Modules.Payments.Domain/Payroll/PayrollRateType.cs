namespace MTK.Modules.Payments.Domain.Payroll;

/// <summary>
/// Əmək haqqından tutulan/işəgötürənin ödədiyi icbari ödənişlərin növü
/// (Azərbaycan qanunvericiliyi: DSMF, işsizlikdən sığorta, icbari tibbi sığorta,
/// gəlir vergisi).
/// </summary>
public enum PayrollRateType
{
    /// <summary>Məcburi dövlət sosial sığorta haqqı (DSMF).</summary>
    Dsmf = 1,
    Unemployment = 2,
    MedicalInsurance = 3,

    /// <summary>Yalnız <see cref="PayrollParty.Employee"/> üçün mənalıdır.</summary>
    IncomeTax = 4
}
