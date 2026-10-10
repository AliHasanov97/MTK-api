using MTK.Common.Domain.Abstractions;

using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.Applications;

public abstract class Application : SearchableEntity
{
    public int ApplicationNumber { get; protected set; }
    public ApplicationType Type { get; protected set; }
    public ApplicationStatus Status { get; protected set; }
    public Guid CreatedById { get; protected set; }
    public User CreatedBy { get; protected set; } = null!;
    public ICollection<FileAttachment> FileAttachments { get; protected set; } = new List<FileAttachment>();

    // Virtual navigation properties for search responses
    public virtual Employee? RelatedEmployee { get; protected set; }
    public virtual JobApplication? RelatedJobApplicant { get; protected set; }
    public virtual Order? RelatedOrder { get; protected set; }

    protected Application() { }

    /// <summary>
    /// Statusu "Əmrə çevrildi" olaraq təyin et
    /// </summary>
    public void MarkAsConvertedToOrder()
    {
        Status = ApplicationStatus.ConvertedToOrder;
    }
}
