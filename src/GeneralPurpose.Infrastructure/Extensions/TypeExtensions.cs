namespace GeneralPurpose.Infrastructure.Extensions;

public static class TypeExtensions
{
    public static bool IsSimpleType(this Type type)
    {
        if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal)) return true;
        
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        
        return underlyingType == typeof(DateTime) ||
               underlyingType == typeof(Guid) ||
               underlyingType == typeof(DateTimeOffset) ||
               underlyingType == typeof(TimeSpan);
    }
}