using System.Dynamic;

namespace ApiTestFramework.Infrastructure.Domain;

public class DynamicJsonObject : DynamicObject
{
    private Dictionary<string, object> _properties = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        string name = binder.Name.ToLower();
        return _properties.TryGetValue(name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        _properties[binder.Name.ToLower()] = value;
        return true;
    }

    public Dictionary<string, object> GetProperties()
    {
        return _properties;
    }

    public object? GetValue(string propertyName)
    {
        return _properties.TryGetValue(propertyName.ToLower(), out var value) ? value : null;
    }

    // Helper to set property without using dynamic binder
    public void Set(string propertyName, object value)
    {
        _properties[propertyName.ToLower()] = value;
    }
}
