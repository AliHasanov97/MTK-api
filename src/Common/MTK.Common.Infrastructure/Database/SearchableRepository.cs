using System.Text.RegularExpressions;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace MTK.Common.Infrastructure.Database;

public class SearchableRepository<T>: Repository<T> where T : SearchableEntity
{
    protected SearchableRepository(DbContext context) : base(context)
    {
    }


    public virtual new async Task<List<T>> SearchAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria,
        string? searchTerm, int? page, int? pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);

        string? searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query
                .Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }
        
        query = ApplyPages(query, page, pageSize);
        return await query.ToListAsync(cancellationToken);
    }
    
    
    public virtual new async Task<int> CountAsync(List<QueryFilter>? filters, SortCriteria? sortCriteria, string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFiltersAndSort(filters, sortCriteria);

        string? searchTermQuery = GetSearchTerm(searchTerm);
        if (!string.IsNullOrWhiteSpace(searchTermQuery))
        {
            query = query
                .Where(p => p.SearchVector.Matches(EF.Functions.ToTsQuery(searchTermQuery)));
        }
        return await query
            .CountAsync(cancellationToken);
    }
    protected static string? GetSearchTerm(string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanTerm = CleanSearchTermRegex(searchTerm);

            if (!string.IsNullOrEmpty(cleanTerm))
            {
                var terms = cleanTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(t => t.Length >= 1) // Ignore very short terms
                    .Select(t => $"{t.ToLower()}:*")
                    .ToList();

                if (terms.Any())
                {
                    var searchTermQuery = string.Join(" | ", terms);

                    return searchTermQuery;
                }
            }
        }

        return null;
    }
    
    private static string CleanSearchTermRegex(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return string.Empty;
    
        // Keep only alphanumeric, spaces, hyphens, and underscores
        var cleaned = Regex.Replace(searchTerm, @"[^\w\s\-]", " ");
    
        // Normalize whitespace
        cleaned = Regex.Replace(cleaned, @"\s+", " ");
    
        return cleaned.Trim();
    }
    
}