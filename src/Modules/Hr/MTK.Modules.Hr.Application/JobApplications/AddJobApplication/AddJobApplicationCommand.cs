using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Application.JobApplications.AddJobApplication;

public sealed class AddJobApplicationCommand : ICommand<AddJobApplicationResponse>
{
    public string Address { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FathersName { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? HomeTelephoneNumber { get; set; }
    public Gender Gender { get; set; }
    public Guid JobId { get; set; }
    public DateTimeOffset StartDate { get; set; }
}
