using HarmonyLib;
using Reptile;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CrewBoom;

public static class LoadUtil
{
    public static readonly BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static Assets GetAssets(CharacterLoader instance)
    {
        var field = instance.GetType().GetField("assets", BindingFlags);
        return (Assets)field?.GetValue(instance);
    }

    public static MethodInfo GetMethod(this object instance, string name)
    {
        return instance.GetType().GetMethod(name, BindingFlags);
    }
    public static void InvokeMethod(this object instance, string name, params object[] parameters)
    {
        Traverse traverse = Traverse.Create(instance);
        traverse = traverse.Method(name, parameters);
        traverse.GetValue(parameters);
    }
    public static void InvokeMethod(this object instance, string name, Type[] types, params object[] parameters)
    {
        Traverse traverse = Traverse.Create(instance);
        traverse = traverse.Method(name, types);
        traverse.GetValue(parameters);
    }

    public static FieldInfo GetField(this object instance, string name)
    {
        return instance.GetType().GetField(name, BindingFlags);
    }
    public static T GetComponentValue<T>(this object instance, string name) where T : UnityEngine.Component
    {
        return GetField(instance, name).GetValue(instance) as T;
    }
    public static T GetFieldValue<T>(this object instance, string name) where T : class
    {
        return GetField(instance, name).GetValue(instance) as T;
    }
    public static void SetField(this object instance, string name, object value)
    {
        instance.GetField(name).SetValue(instance, value);
    }
}
