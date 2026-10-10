using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition;

public static class ApplicationForChangeOfPositionErrors
{
    public static readonly Error EmployeeNotFound = new Error(
        "ApplicationForChangeOfPosition.EmployeeNotFound",
        "The employee was not found");

    public static readonly Error CompanyNotFound = new Error(
        "ApplicationForChangeOfPosition.CompanyNotFound",
        "The company was not found");

    public static readonly Error JobNotFound = new Error(
        "ApplicationForChangeOfPosition.JobNotFound",
        "The department job was not found");

    public static readonly Error EmployeeCompanyMismatch = new Error(
        "ApplicationForChangeOfPosition.EmployeeCompanyMismatch",
        "The employee does not belong to the specified company");

    public static readonly Error SamePosition = new Error(
        "ApplicationForChangeOfPosition.SamePosition",
        "Current and new positions must be different");

    public static readonly Error NotFound = new Error(
        "ApplicationForChangeOfPosition.NotFound",
        "The application for change of position was not found");

    public static readonly Error AlreadyConverted = new Error(
        "ApplicationForChangeOfPosition.AlreadyConverted",
        "The application has already been converted to an order");

    public static readonly Error AddFailed = new Error(
        "ApplicationForChangeOfPosition.AddFailed",
        "Vəzifə dəyişikliyi ərizəsi əlavə edilərkən xəta baş verdi");

    public static readonly Error UpdateFailed = new Error(
        "ApplicationForChangeOfPosition.UpdateFailed",
        "Vəzifə dəyişikliyi ərizəsi yenilənərkən xəta baş verdi");
}
