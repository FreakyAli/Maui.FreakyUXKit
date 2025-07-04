using System;

namespace Samples;

public static class Extensions
{
    public static T GetResource<T>(this ResourceDictionary dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value) && value is T resource)
            return resource;
        else
            return default;
    }
}
