using Reptile;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BrcCustomCharacters;

public static class LoadUtil
{
    private static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static Assets GetAssets(CharacterLoader instance)
    {
        var field = instance.GetType().GetField("assets", bindingFlags);
        return (Assets)field?.GetValue(instance);
    }

    public static MethodInfo GetMethod(this object instance, string name)
    {
        return instance.GetType().GetMethod(name, bindingFlags);
    }
    public static void InvokeMethod(this object instance, string name, params object[] parameters)
    {
        instance.GetMethod(name).Invoke(instance, parameters);
    }

    public static FieldInfo GetField(this object instance, string name)
    {
        return instance.GetType().GetField(name, bindingFlags);
    }
    public static T GetFieldValue<T>(this object instance, string name) where T : UnityEngine.Component
    {
        return GetField(instance, name).GetValue(instance) as T;
    }
    public static void SetField(this object instance, string name, object value)
    {
        instance.GetField(name).SetValue(instance, value);
    }
}
