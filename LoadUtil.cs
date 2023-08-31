using Reptile;
using System.Reflection;

namespace BrcCustomCharacters;

public static class LoadUtil
{
    private static BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static Assets GetAssets(CharacterLoader instance)
    {
        var field = instance.GetType().GetField("assets", bindingFlags);
        return (Assets)field?.GetValue(instance);
    }

    public static MethodInfo GetMethod(string name, CharacterLoader instance)
    {
        return instance.GetType().GetMethod(name, bindingFlags);
    }

    public static FieldInfo GetGraffitiInfo(GraffitiLoader instance)
    {
        return instance.GetType().GetField("graffitiArtInfo", bindingFlags);
    }

    public static FieldInfo GetField(string name, object instance)
    {
        return instance.GetType().GetField(name, bindingFlags);
    }
}
