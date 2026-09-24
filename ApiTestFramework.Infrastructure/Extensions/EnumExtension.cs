using System.ComponentModel;
using System.Reflection;

namespace ApiTestFramework.Infrastructure.Extensions;

public static class EnumExtension
{
    public static string GetDescription(this System.Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    public static IEnumerable<(T Value, string Description)> GetAllDescriptions<T>() where T : struct, Enum
    {
        foreach (T value in Enum.GetValues<T>())
        {
            yield return (value, value.GetDescription());
        }
    }
}
