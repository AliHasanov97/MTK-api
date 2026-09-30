namespace MTK.Modules.Payments.Domain.Contracts;

public enum ContractStatus
{
    Draft,        // Hazırlanır — maddələr/xidmətlər dəyişdirilə bilər
    Active,       // Qüvvədədir
    Suspended,    // Dayandırılıb
    Terminated    // Vaxtından əvvəl ləğv edilib
    // "Expired" yoxdur: müddətin bitməsi Contract.IsExpired ilə hesablanır,
    // ona görə status-u vaxtı çatanda dəyişən ayrıca job lazım olmur.
}
