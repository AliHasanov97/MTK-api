using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.Applications;

using MTK.Modules.Hr.Domain.EmploymentOrders;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.JobApplications.Events;
using MTK.Modules.Hr.Domain.Jobs;
using System.Text.Json.Serialization;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Domain.JobApplications;

public sealed class JobApplication : Application
{
    public string Address { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string FathersName { get; private set; } = string.Empty;
    public string Telephone { get; private set; } = string.Empty;
    public string? HomeTelephoneNumber { get; private set; }
    public Gender Gender { get; private set; }
    public Guid JobId { get; private set; }
    public Job Job { get; private set; } = null!;
    public DateTimeOffset StartDate { get; private set; }

    /// <summary>
    /// Bu ərizədən yaradılmış əmr (nullable - convert olunmamış ərizələrdə null ola bilər)
    /// </summary>
    public EmploymentOrder? EmploymentOrder { get; private set; }

    // Override base properties
    [JsonIgnore]
    public override JobApplication? RelatedJobApplicant => this;
    public override Order? RelatedOrder => EmploymentOrder;

    private JobApplication() { }

    public static JobApplication Create(
        string address,
        string name,
        string surname,
        string fathersName,
        string telephone,
        string? homeTelephoneNumber,
        Gender gender,
        Guid jobId,
        DateTimeOffset startDate,
        Guid createdById)
    {
        var id = Guid.NewGuid();
        var jobApplication = new JobApplication
        {
            Id = id,
            Type = ApplicationType.JobApplication,
            Status = ApplicationStatus.PendingApproval,
            CreatedById = createdById,
            Address = address,
            Name = name,
            Surname = surname,
            FathersName = fathersName,
            Telephone = telephone,
            HomeTelephoneNumber = homeTelephoneNumber,
            Gender = gender,
            JobId = jobId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate)
        };

        jobApplication.RaiseDomainEvent(new JobApplicationCreatedDomainEvent
        {
            JobApplicationId = jobApplication.Id,
            ApplicationDate = startDate
        });

        return jobApplication;
    }

    public void Update(
        string? address,
        string? name,
        string? surname,
        string? fathersName,
        string? telephone,
        string? homeTelephoneNumber,
        Gender? gender,
        Guid? jobId,
        DateTimeOffset? startDate)
    {
        if (address is not null)      Address = address;
        if (name is not null)         Name = name;
        if (surname is not null)      Surname = surname;
        if (fathersName is not null)  FathersName = fathersName;
        if (telephone is not null)    Telephone = telephone;
        if (gender.HasValue)          Gender = gender.Value;
        if (jobId.HasValue) JobId = jobId.Value;
        if (startDate.HasValue)       StartDate = DateTimeHelper.ToUtcDateOnly(startDate.Value);

        if (homeTelephoneNumber is not null)
            HomeTelephoneNumber = string.IsNullOrWhiteSpace(homeTelephoneNumber) ? null : homeTelephoneNumber;
    }
}
