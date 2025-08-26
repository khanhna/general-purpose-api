using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using GeneralPurpose.Domain.Models;
using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Persistence;
using GeneralPurpose.Infrastructure.Utilities;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurpose.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> WhereIf<TSource>([NotNull] this IQueryable<TSource> query, bool condition,
        Expression<Func<TSource, bool>> whereExpression) where TSource : class
    {
        return condition ? query.Where(whereExpression) : query;
    }

    public static IQueryable<TSource> OrderIf<TSource>([NotNull] this IQueryable<TSource> query, bool condition,
        Expression<Func<TSource, DateTime>> orderExpression) where TSource : class
    {
        return condition ? query.OrderBy(orderExpression) : query;
    }

    public static IQueryable<TSource> OrderDescendingIf<TSource>([NotNull] this IQueryable<TSource> query, bool condition,
        Expression<Func<TSource, DateTime>> orderExpression) where TSource : class
    {
        return condition ? query.OrderByDescending(orderExpression) : query;
    }

    public static IQueryable<TSource> OrderIf<TSource, TKey>([NotNull] this IQueryable<TSource> query, bool condition,
        Expression<Func<TSource, TKey>> orderExpression) where TSource : class
    {
        return condition ? query.OrderBy(orderExpression) : query;
    }

    public static IQueryable<TSource> OrderDescendingIf<TSource, TKey>([NotNull] this IQueryable<TSource> query, bool condition,
        Expression<Func<TSource, TKey>> orderExpression) where TSource : class
    {
        return condition ? query.OrderByDescending(orderExpression) : query;
    }
    
    public static IQueryable<TSource> AsNoTrackingIf<TSource>(this IQueryable<TSource> query, bool condition) where TSource : class
    {
        return condition ? query.AsNoTracking() : query;
    }

    public static IQueryable<TSource> ApplyQueryTranslate<TSource>(this IQueryable<TSource> query, IEFQueryTranslate<TSource> condition)
        where TSource : IAggregateRoot
    {
        return condition.TranslateToQuery(query);
    }

    public static IQueryable<TSource> ApplySortingAndPaging<TSource>(this IQueryable<TSource> query, IPagingIndicate? request)
        where TSource : IAggregateRoot
    {
        if (request == null)
            return query;

        if (request.OrderBy.Length == 0)
        {
            if (request.PageNo.HasValue && request.PageSize.HasValue)
                query = query.Skip((request.PageNo.Value - 1) * request.PageSize.Value)
                    .Take(request.PageSize.Value);
            return query;
        }

        IOrderedQueryable<TSource>? orderedQueryable = null;
        var expressionOrders = QueryableUtil.DynamicSortingBuilderExpressions<TSource>(request.OrderBy);
        var isFirstOrderImplement = true;
        while (expressionOrders.TryDequeue(out var orderExpression))
        {
            if (orderExpression.Item1 == null) continue;

            if (isFirstOrderImplement)
                orderedQueryable = orderExpression.Item2
                    ? query.OrderBy(orderExpression.Item1)
                    : query.OrderByDescending(orderExpression.Item1);
            else
                orderedQueryable = orderExpression.Item2
                    ? orderedQueryable?.ThenBy(orderExpression.Item1)
                    : orderedQueryable?.ThenByDescending(orderExpression.Item1);

            isFirstOrderImplement = false;
        }

        query = orderedQueryable ?? query;

        if (request.PageNo.HasValue && request.PageSize.HasValue)
            query = query.Skip(request.PageNo.Value * request.PageSize.Value).Take(request.PageSize.Value);

        return query;
    }
}