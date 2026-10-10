using MTK.Common.Application.Messaging;
using MTK.Modules.Hr.Domain.JobApplications;
using System.Text.Json.Serialization;

namespace MTK.Modules.Hr.Application.JobApplications.UpdateJobApplication;

public sealed class UpdateJobApplicationCommand : ICommand<UpdateJobApplicationResponse>
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public string? Address { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? FathersName { get; set; }
    public string? Telephone { get; set; }
    public string? HomeTelephoneNumber { get; set; }
    public Gender? Gender { get; set; }
    public Guid? JobId { get; set; }
    public DateTimeOffset? StartDate { get; set; }
}
