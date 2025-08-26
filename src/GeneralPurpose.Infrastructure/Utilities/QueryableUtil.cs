using System.Linq.Expressions;
using GeneralPurpose.Domain.Constants;
using GeneralPurpose.Infrastructure.Extensions;

namespace GeneralPurpose.Infrastructure.Utilities;

public static class QueryableUtil
{
    public static Queue<Tuple<Expression<Func<TSource, object>>, bool>> DynamicSortingBuilderExpressions<TSource>(string[] properties)
    {
        // Not support sorting by property of child(nested) collection
        var result = new Queue<Tuple<Expression<Func<TSource, object>>, bool>>();
        if (properties == null || !properties.Any())
        {
            return result;
        }
        
        var paramExpression = Expression.Parameter(typeof(TSource));
        for (var i = 0; i < properties.Length; i++)
        {
            if (properties[i] == CommonConstants.CustomReplacementDynamicOrderBy)
            {
                result.Enqueue(new Tuple<Expression<Func<TSource, object>>, bool>(null, default));
                continue;
            }

            var isAscendingOrder = true;
            var orderParams = properties[i].Trim().Split(CommonConstants.DefaultClientDelimiter);
            if (!orderParams.TryGetElement(0, out var fieldName)) continue;

            var accessor = fieldName.Contains('.')
                ? NestedExpressionConverter(paramExpression, fieldName)
                : Expression.PropertyOrField(paramExpression, fieldName);
            var conversion = Expression.Convert(accessor, typeof(object));
            if (orderParams.TryGetElement(1, out var orderType))
            {
                isAscendingOrder = string.Compare(orderType, "desc", StringComparison.OrdinalIgnoreCase) != 0;
            }

            result.Enqueue(new Tuple<Expression<Func<TSource, object>>, bool>(
                Expression.Lambda<Func<TSource, object>>(conversion, false, paramExpression), isAscendingOrder));
        }
        return result;
    }

    private static MemberExpression NestedExpressionConverter(ParameterExpression paramExpression, string propertyName)
    {
        var body = propertyName.Split('.')
            .Aggregate<string, Expression>(paramExpression, Expression.PropertyOrField);
        return body as MemberExpression;
    }
}