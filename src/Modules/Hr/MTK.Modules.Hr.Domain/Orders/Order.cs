using MTK.Common.Domain.Abstractions;

using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.JobApplications;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.Orders;

public abstract class Order : SearchableEntity
{
    public int OrderNumber { get; protected set; }
    public OrderType Type { get; protected set; }
    public Guid CreatedById { get; protected set; }
    public User CreatedBy { get; protected set; } = null!;
    public ICollection<FileAttachment> FileAttachments { get; protected set; } = new List<FileAttachment>();

    // Virtual navigation properties for search responses
    public virtual Employee? RelatedEmployee { get; protected set; }
    public virtual JobApplication? RelatedJobApplicant { get; protected set; }

    protected Order() { }
}
