using System.Dynamic;

public class DynamicClass : DynamicObject
{
    Dictionary<string, object> properties = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return properties.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        properties[binder.Name] = value;
        return true;
    }
}


public class DynamicClass2 : DynamicObject
{
    private Dictionary<string, object> properties = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        return properties.TryGetValue(binder.Name, out result);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        properties[binder.Name] = value;
        return true;
    }

    // الميثود اللي بتضيف خصائص جديدة باستخدام اسم وقيمة
    public void AddProperty(string name, object value)
    {
        properties[name] = value;
    }

    // ميثود لتحويل الخصائص لـ Dictionary عشان JSON serializer يقدر يقرأها
    public Dictionary<string, object> ToDictionary()
    {
        return properties;
    }
}
