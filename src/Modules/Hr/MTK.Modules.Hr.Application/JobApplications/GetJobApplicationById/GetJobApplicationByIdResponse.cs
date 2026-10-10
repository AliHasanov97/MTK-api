using MTK.Common.Presentation.Responses;
using MTK.Modules.Hr.Application.FileAttachments;

namespace MTK.Modules.Hr.Application.JobApplications.GetJobApplicationById;

public class GetJobApplicationByIdResponse
{
    public Guid Id { get; set; }
    public int ApplicationNumber { get; set; }
    public string Status { get; set; } = null!;
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? HomeTelephoneNumber { get; set; }
    public string Gender { get; set; } = null!;
    public ResponseObjectWithName? Job { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public ResponseObjectWithName? EmploymentOrder { get; set; }
    public ResponseObjectWithName CreatedBy { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
