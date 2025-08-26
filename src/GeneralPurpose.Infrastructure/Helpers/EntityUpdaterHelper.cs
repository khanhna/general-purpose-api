using System.Reflection;
using GeneralPurpose.Domain.SeedWork;
using GeneralPurpose.Infrastructure.Extensions;

namespace GeneralPurpose.Infrastructure.Helpers;

public static class EntityUpdaterHelper
{
    private static readonly Dictionary<Type, Dictionary<string, (bool shouldUpdate, PropertyInfo propertyInfo)>>
        EntityPropertiesMap = new();

    private static readonly HashSet<string> ExcludedProperties = new()
    {
        nameof(Entity<int>.Id),
        nameof(Entity<int>.CreatedBy),
        nameof(Entity<int>.CreatedTime),
        nameof(Entity<int>.LastUpdatedBy),
        nameof(Entity<int>.LastUpdatedTime)
    };
    
    public static bool Update<TEntity, T>(TEntity? entity, T? incomingModel)
        where TEntity : BaseEntity
        where T : class
    {
        var result = false;
        if(entity == null || incomingModel == null) return result;
        
        if (!EntityPropertiesMap.TryGetValue(typeof(TEntity), out var entityPropertiesInfo))
        {
            var targetType = typeof(TEntity); 
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            entityPropertiesInfo = properties.Where(x => !ExcludedProperties.Contains(x.Name)).ToDictionary(x => x.Name,
                x => (x.PropertyType.IsSimpleType() && x is { CanRead: true, CanWrite: true }, x));
            EntityPropertiesMap.Add(targetType, entityPropertiesInfo);
        }
        
        if (!EntityPropertiesMap.TryGetValue(typeof(T), out var updatePropertiesInfo))
        {
            var targetType = typeof(T);
            var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            updatePropertiesInfo = properties.Where(x => !ExcludedProperties.Contains(x.Name)).ToDictionary(x => x.Name,
                x => (x.PropertyType.IsSimpleType() && x is { CanRead: true, CanWrite: true }, x));
            EntityPropertiesMap.Add(targetType, updatePropertiesInfo);
        }

        foreach (var updateProperty in updatePropertiesInfo)
        {
            if (!entityPropertiesInfo.TryGetValue(updateProperty.Key, out var entityProperty) ||
                !entityProperty.shouldUpdate) continue;

            var currentValue = entityProperty.propertyInfo.GetValue(entity);
            var newValue = updateProperty.Value.propertyInfo.GetValue(incomingModel);

            if (!Equals(currentValue, newValue))
            {
                entityProperty.propertyInfo.SetValue(entity, newValue);
                result = true;
            }
        }

        return result;
    }
}