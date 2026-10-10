using MTK.Common.Domain.Abstractions;

using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.FileAttachments;
using MTK.Modules.Hr.Domain.Helpers;
using MTK.Modules.Hr.Domain.Users;

namespace MTK.Modules.Hr.Domain.NoticesOfChangeInWorkingConditions;

public sealed class NoticeOfChangeInWorkingConditions : SearchableEntity
{
    public int Index { get; private set; }
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = null!;
    public DateTimeOffset StartDate { get; private set; }
    public Guid CreatedById { get; private set; }
    public User CreatedBy { get; private set; } = null!;
    public ICollection<FileAttachment> FileAttachments { get; private set; } = new List<FileAttachment>();

    private NoticeOfChangeInWorkingConditions()
    {
    }

    public static NoticeOfChangeInWorkingConditions Create(
        int index,
        Guid employeeId,
        DateTimeOffset startDate,
        Guid createdById)
    {
        return new NoticeOfChangeInWorkingConditions
        {
            Id = Guid.NewGuid(),
            Index = index,
            EmployeeId = employeeId,
            StartDate = DateTimeHelper.ToUtcDateOnly(startDate),
            CreatedById = createdById
        };
    }
}