using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.SalaryPayments.Queries.SearchSalaryPayments;

internal sealed class SearchSalaryPaymentsQueryHandler
    : IQueryHandler<SearchSalaryPaymentsQuery, SearchSalaryPaymentsResponse>
{
    private readonly ISalaryPaymentRepository _salaryPaymentRepository;

    public SearchSalaryPaymentsQueryHandler(ISalaryPaymentRepository salaryPaymentRepository)
    {
        _salaryPaymentRepository = salaryPaymentRepository;
    }

    public async Task<Result<SearchSalaryPaymentsResponse>> Handle(
        SearchSalaryPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        var salaryPayments = await _salaryPaymentRepository.SearchAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _salaryPaymentRepository.CountAsync(
            request.Filters,
            request.SortCriteria,
            request.SearchTerm,
            cancellationToken);

        var items = salaryPayments.Select(s => new SalaryPaymentSearchResult(
            s.Id,
            s.EmployeeId,
            s.EmployeeFullName,
            s.SalaryMonth,
            s.SalaryYear,
            s.GrossSalary,
            s.PaymentMethod,
            s.PaymentDate,
            s.Status,
            s.CreatedAt)).ToList();

        return Result.Success(new SearchSalaryPaymentsResponse(
            items,
            totalCount,
            request.Page ?? 1,
            request.PageSize ?? 10));
    }
}
