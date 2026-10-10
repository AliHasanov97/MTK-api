using MTK.Common.Presentation.Responses;

namespace MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;

public class UpdateJobApplicationResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? HomeTelephoneNumber { get; set; }
    public string Gender { get; set; } = string.Empty;
    public ResponseObjectWithName? Job { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
