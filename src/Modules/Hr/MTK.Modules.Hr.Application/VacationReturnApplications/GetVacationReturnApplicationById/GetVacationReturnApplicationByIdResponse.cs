using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.GetVacationReturnApplicationById;

public sealed class GetVacationReturnApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public ResponseObjectWithName Employee { get; set; } = null!;
    public DateTimeOffset ReturnDate { get; set; }
    public string? Notes { get; set; }
    public ResponseObjectWithName? VacationReturnOrder { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
}