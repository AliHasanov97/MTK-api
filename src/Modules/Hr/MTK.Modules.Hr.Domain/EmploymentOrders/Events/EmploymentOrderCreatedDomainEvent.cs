using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Domain.JobApplications;

namespace MTK.Modules.Hr.Domain.EmploymentOrders.Events;

public sealed record EmploymentOrderCreatedDomainEvent : DomainEvent
{
    public Guid EmploymentOrderId { get; init; }
    public int OrderNumber { get; init; }
    public DateTimeOffset OrderDate { get; init; }

    // Employee data — carried from JobApplication at create time
    public string Name { get; init; } = string.Empty;
    public string Surname { get; init; } = string.Empty;
    public string FathersName { get; init; } = string.Empty;
    public Gender Gender { get; init; }
    public Guid JobApplicationId { get; init; }
    public Guid JobId { get; init; }
    public Guid CreatedById { get; init; }
}
