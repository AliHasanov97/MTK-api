using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using MTK.Common.Domain.Abstractions;
using MTK.Common.Domain.Queries;

namespace MTK.Common.Infrastructure.Database;
public static class QueryHelper
{
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<QueryFilter>? filters)
    {
        if (filters == null || !filters.Any())
            return query;

        Expression<Func<T, bool>>? combinedExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression<T>(filter);
            if (filterExpression != null)
            {
                combinedExpression = combinedExpression == null 
                    ? filterExpression 
                    : CombineExpressions(combinedExpression, filterExpression, ExpressionType.AndAlso);
            }
        }

        return combinedExpression != null ? query.Where(combinedExpression) : query;
    }

    public static IQueryable<T> ApplyFiltersWithOr<T>(this IQueryable<T> query, List<QueryFilter>? filters)
    {
        if (filters == null || !filters.Any())
            return query;

        Expression<Func<T, bool>>? combinedExpression = null;

        foreach (var filter in filters)
        {
            var filterExpression = BuildFilterExpression<T>(filter);
            if (filterExpression != null)
            {
                combinedExpression = combinedExpression == null 
                    ? filterExpression 
                    : CombineExpressions(combinedExpression, filterExpression, ExpressionType.OrElse);
            }
        }

        return combinedExpression != null ? query.Where(combinedExpression) : query;
    }
    
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, List<SortCriteria>? sortCriteria)
    {
        if (sortCriteria == null || !sortCriteria.Any())
            return query;

        IOrderedQueryable<T>? orderedQuery = null;
        bool isFirst = true;

        foreach (var sort in sortCriteria)
        {
            var propertyExpression = BuildSortExpression<T>(sort.ColumnName);
            if (propertyExpression == null)
                continue;

            if (isFirst)
            {
                orderedQuery = sort.Direction == SortDirection.Ascending
                    ? query.OrderBy(propertyExpression)
                    : query.OrderByDescending(propertyExpression);
                isFirst = false;
            }
            else
            {
                orderedQuery = sort.Direction == SortDirection.Ascending
                    ? orderedQuery!.ThenBy(propertyExpression)
                    : orderedQuery!.ThenByDescending(propertyExpression);
            }
        }

        return orderedQuery ?? query;
    }
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortCriteria? sortCriteria)
    {
        if (sortCriteria == null)
        {
            if(typeof(T).IsAssignableTo(typeof(IAuditable)))
            {
                return query.OrderByDescending(x => ((IAuditable)x!).CreatedAt);
            }
            return query;
        }

        return query.ApplySort(new List<SortCriteria> { sortCriteria });
    }
    
    public static IQueryable<T> ApplyFiltersAndSort<T>(this IQueryable<T> query, List<QueryFilter>? filters, List<SortCriteria>? sortCriteria)
    {
        return query.ApplyFilters(filters).ApplySort(sortCriteria);
    }
    public static IQueryable<T> ApplyFiltersAndSort<T>(this IQueryable<T> query, List<QueryFilter>? filters, SortCriteria? sortCriteria)
    {
        return query.ApplyFilters(filters).ApplySort(sortCriteria);
    }
    public static IQueryable<T> ApplyFiltersAndSort<T>(this IQueryable<T> query, List<QueryFilter>? filters, string sortColumn, SortDirection sortDirection = SortDirection.Ascending)
    {
        var sortCriteria = new List<SortCriteria>
        {
            new SortCriteria { ColumnName = sortColumn, Direction = sortDirection }
        };
        return query.ApplyFilters(filters).ApplySort(sortCriteria);
    }

    private static Expression<Func<T, object>>? BuildSortExpression<T>(string propertyPath)
    {
        try
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = GetPropertyExpression(parameter, propertyPath);
            
            if (property == null)
                return null;

            // Convert to object for consistent return type
            var converted = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(converted, parameter);
        }
        catch
        {
            return null;
        }
    }

    private static Expression<Func<T, bool>>? BuildFilterExpression<T>(QueryFilter filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        // Check if this is a collection path (contains a collection in the path)
        var collectionExpression = TryBuildCollectionFilterExpression<T>(parameter, filter);
        if (collectionExpression != null)
            return collectionExpression;

        var property = GetPropertyExpression(parameter, filter.ColumnName);

        if (property == null)
            return null;

        var filterExpression = CreateComparisonExpression(property, filter.Comparison, filter.Value);

        return filterExpression != null
            ? Expression.Lambda<Func<T, bool>>(filterExpression, parameter)
            : null;
    }

    private static Expression? GetPropertyExpression(Expression parameter, string propertyPath)
    {
        try
        {
            var properties = propertyPath.Split('.');
            Expression property = parameter;

            foreach (var prop in properties)
            {
                var propertyInfo = property.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    return null;

                property = Expression.Property(property, propertyInfo);
            }

            return property;
        }
        catch
        {
            return null;
        }
    }

    private static Expression<Func<T, bool>>? TryBuildCollectionFilterExpression<T>(ParameterExpression parameter, QueryFilter filter)
    {
        try
        {
            var properties = filter.ColumnName.Split('.');
            Expression currentExpression = parameter;

            for (int i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                var propertyInfo = currentExpression.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                    return null;

                var propertyType = propertyInfo.PropertyType;

                // Check if this property is a collection (ICollection<T>, IEnumerable<T>, List<T>, etc.)
                var collectionElementType = GetCollectionElementType(propertyType);
                if (collectionElementType != null && i < properties.Length - 1)
                {
                    // This is a collection and there are more properties after it
                    var collectionProperty = Expression.Property(currentExpression, propertyInfo);
                    var remainingPath = string.Join(".", properties.Skip(i + 1));

                    // Build the inner lambda for .Any()
                    var innerParameter = Expression.Parameter(collectionElementType, "inner");
                    var innerProperty = GetPropertyExpression(innerParameter, remainingPath);

                    if (innerProperty == null)
                        return null;

                    var innerComparison = CreateComparisonExpression(innerProperty, filter.Comparison, filter.Value);
                    if (innerComparison == null)
                        return null;

                    var innerLambda = Expression.Lambda(innerComparison, innerParameter);

                    // Build the .Any() call
                    var anyMethod = typeof(Enumerable).GetMethods()
                        .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                        .MakeGenericMethod(collectionElementType);

                    var anyCall = Expression.Call(anyMethod, collectionProperty, innerLambda);

                    return Expression.Lambda<Func<T, bool>>(anyCall, parameter);
                }

                currentExpression = Expression.Property(currentExpression, propertyInfo);
            }

            // No collection found in path, return null to use standard processing
            return null;
        }
        catch
        {
            return null;
        }
    }

    private static Type? GetCollectionElementType(Type type)
    {
        // Check for ICollection<T>, IEnumerable<T>, List<T>, etc.
        if (type.IsGenericType)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (genericDef == typeof(ICollection<>) ||
                genericDef == typeof(IEnumerable<>) ||
                genericDef == typeof(IList<>) ||
                genericDef == typeof(List<>) ||
                genericDef == typeof(HashSet<>))
            {
                return type.GetGenericArguments()[0];
            }
        }

        // Check implemented interfaces
        foreach (var iface in type.GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                return iface.GetGenericArguments()[0];
            }
        }

        return null;
    }
    private static Expression? CreateComparisonExpression(Expression property, object value, 
        Func<Expression, Expression, BinaryExpression> comparisonFunc)
    {
        var constant = CreateConstantExpression(property.Type, value);
        
        return constant != null ? comparisonFunc(property, constant) : null;
    }

    private static Expression? CreateComparisonExpression(Expression property, QueryComparisonType comparison, object value)
    {
        try
        {
            switch (comparison)
            {
                case QueryComparisonType.Equals:
                    return CreateEqualsExpression(property, value);

                case QueryComparisonType.NotEquals:
                    var equalsExpr = CreateEqualsExpression(property, value);
                    return equalsExpr != null ? Expression.Not(equalsExpr) : null;

                case QueryComparisonType.Contains:
                    return CreateStringMethodExpression(property, "Contains", value);

                case QueryComparisonType.StartsWith:
                    return CreateStringMethodExpression(property, "StartsWith", value);

                case QueryComparisonType.EndsWith:
                    return CreateStringMethodExpression(property, "EndsWith", value);

                case QueryComparisonType.GreaterThan:
                    return CreateComparisonExpression(property, value, Expression.GreaterThan);

                case QueryComparisonType.GreaterThanOrEqual:
                    return CreateComparisonExpression(property, value, Expression.GreaterThanOrEqual);

                case QueryComparisonType.LessThan:
                    return CreateComparisonExpression(property, value, Expression.LessThan);

                case QueryComparisonType.LessThanOrEqual:
                    return CreateComparisonExpression(property, value, Expression.LessThanOrEqual);

                case QueryComparisonType.IsNull:
                    return Expression.Equal(property, Expression.Constant(null));

                case QueryComparisonType.IsNotNull:
                    return Expression.NotEqual(property, Expression.Constant(null));

                case QueryComparisonType.In:
                    return CreateInExpression(property, value);

                case QueryComparisonType.NotIn:
                    var inExpr = CreateInExpression(property, value);
                    return inExpr != null ? Expression.Not(inExpr) : null;

                default:
                    return null;
            }
        }
        catch
        {
            return null;
        }
    }

    private static Expression? CreateEqualsExpression(Expression property, object value) //NOSONAR
    {
        var constant = CreateConstantExpression(property.Type, value);
        return constant != null ? Expression.Equal(property, constant) : null;
    }

    private static MethodCallExpression? CreateStringMethodExpression(Expression property, string methodName, object? value)
    {
        if (property.Type != typeof(string) || value == null)
            return null;

        var method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
        if (method == null)
            return null;

        var constant = Expression.Constant(value.ToString());
        return Expression.Call(property, method, constant);
    }


    private static MethodCallExpression? CreateInExpression(Expression property, object? value)
    {
        if (value == null)
            return null;

        var valueType = value.GetType();

        // Handle JsonElement arrays
        if (value is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
        {
            var listType = typeof(List<>).MakeGenericType(property.Type);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");

            if (list == null || addMethod == null)
                return null;

            foreach (var item in jsonElement.EnumerateArray())
            {
                var convertedValue = ConvertValue(item, property.Type);
                if (convertedValue != null)
                {
                    addMethod.Invoke(list, new[] { convertedValue });
                }
            }

            value = list;
        }
        else if (!valueType.IsGenericType || !valueType.GetGenericTypeDefinition().Equals(typeof(List<>)))
        {
            // Try to convert single value to list
            var listType = typeof(List<>).MakeGenericType(property.Type);
            var list = Activator.CreateInstance(listType);
            var addMethod = listType.GetMethod("Add");
            var convertedValue = ConvertValue(value, property.Type);
            if (convertedValue != null && addMethod != null && list != null)
            {
                addMethod.Invoke(list, new[] { convertedValue });
                value = list;
            }
            else
            {
                return null;
            }
        }

        var containsMethod = value.GetType().GetMethod("Contains");
        if (containsMethod == null)
            return null;

        var constant = Expression.Constant(value);
        return Expression.Call(constant, containsMethod, property);
    }

    private static Expression? CreateConstantExpression(Type targetType, object? value)
    {
        if (value == null)
            return Expression.Constant(null, targetType);

        var convertedValue = ConvertValue(value, targetType);
        return convertedValue != null ? Expression.Constant(convertedValue, targetType) : null;
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        try
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            
            if (value.GetType() == underlyingType)
                return value;

            // Handle string/JsonElement to enum conversion
            if (underlyingType.IsEnum)
            {
                string? enumString = value switch
                {
                    JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
                    JsonElement je when je.ValueKind == JsonValueKind.Number => je.GetInt32().ToString(),
                    string s => s,
                    _ => value?.ToString()
                };
                if (!string.IsNullOrWhiteSpace(enumString))
                    return Enum.Parse(underlyingType, enumString, true);
            }

            // Handle string to Guid conversion
            if (underlyingType == typeof(Guid))
            {
                string? guidString = value switch
                {
                    JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
                    string s => s,
                    _ => null
                };
                if (!string.IsNullOrWhiteSpace(guidString))
                {
                    return Guid.Parse(guidString);
                }
            }

            // Handle JsonElement to string conversion
            if (underlyingType == typeof(string) && value is JsonElement strElement)
            {
                return strElement.ValueKind == JsonValueKind.String
                    ? strElement.GetString()
                    : strElement.ToString();
            }

            // General conversion
            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            return null;
        }
    }

    private static Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        ExpressionType expressionType)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        
        var firstBody = ReplaceParameter(first.Body, first.Parameters[0], parameter);
        var secondBody = ReplaceParameter(second.Body, second.Parameters[0], parameter);
        
        var combined = expressionType == ExpressionType.AndAlso
            ? Expression.AndAlso(firstBody, secondBody)
            : Expression.OrElse(firstBody, secondBody);
            
        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression oldParameter, ParameterExpression newParameter)
    {
        return new ParameterReplacer(oldParameter, newParameter).Visit(expression);
    }

    private sealed class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacer(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : node;
        }
    }
}
