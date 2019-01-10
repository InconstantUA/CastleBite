using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


// Not used, keep as example
[Serializable]
public class CityIDEnumerable : IComparable
{
    // this must appear before other static instance types.
    public static List<CityIDEnumerable> AllCities { get; } = new List<CityIDEnumerable>();

    public static CityIDEnumerable Stormberg { get; } = new CityIDEnumerable(0, "Stormberg");
    public static CityIDEnumerable Zrurrugh { get; }  = new CityIDEnumerable(1, "Zrurrugh");
    public static CityIDEnumerable SintJoris { get; } = new CityIDEnumerable(2, "Sint Joris");
    public static CityIDEnumerable Nartley { get; }   = new CityIDEnumerable(3, "Nartley");

    public string displayName;
    public int Value { get; private set; }

    private CityIDEnumerable(int val, string name)
    {
        Value = val;
        displayName = name;
        AllCities.Add(this);
    }

    public override string ToString()
    {
        return displayName;
    }

    public static IEnumerable<T> GetAll<T>() where T : CityIDEnumerable, new()
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            var instance = new T();
            var locatedValue = info.GetValue(instance) as T;

            if (locatedValue != null)
            {
                yield return locatedValue;
            }
        }
    }

    public override bool Equals(object obj)
    {
        var otherValue = obj as CityIDEnumerable;

        if (otherValue == null)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Value.Equals(otherValue.Value);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static int AbsoluteDifference(CityIDEnumerable firstValue, CityIDEnumerable secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
        return absoluteDifference;
    }

    public static T FromValue<T>(int value) where T : CityIDEnumerable, new()
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Value == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : CityIDEnumerable, new()
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.displayName == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : CityIDEnumerable, new()
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
            throw new ApplicationException(message);
        }

        return matchingItem;
    }

    public int CompareTo(object other)
    {
        return Value.CompareTo(((CityIDEnumerable)other).Value);
    }
}