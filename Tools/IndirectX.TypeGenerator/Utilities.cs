using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndirectX.TypeGenerator;

internal static class Utilities
{
    private static Dictionary<string, Regex> RegexCache = new();

    public static string StringJoin<T>(this IEnumerable<T> values, string separator)
    {
        return string.Join(separator, values);
    }

    public static string Replace(this string input, Regex regex, string replacement)
    {
        return regex.Replace(input, replacement);
    }

    public static string Replace(this string input, Regex regex, MatchEvaluator evaluator)
    {
        return regex.Replace(input, evaluator);
    }

    public static bool IsMatchAll(this Regex regex, string input)
    {
        var m = regex.Match(input);
        return m.Success && m.Index == 0 && m.Length == input.Length;
    }

    public static bool IsMatchAll(this string? regexPattern, string input)
    {
        if (regexPattern is null) return false;

        if (!RegexCache.TryGetValue(regexPattern, out var regex))
        {
            regex = new Regex(regexPattern);
            RegexCache.Add(regexPattern, regex);
        }

        return regex.IsMatchAll(input);
    }

    public static bool IsSnake(this string str) => !str.Any(c => 'a' <= c && c <= 'z');

    public static bool IsInterfacePointer(this string typeName) =>
        typeName.StartsWith('I') && typeName.EndsWith('*') && !typeName.IsSnake();

    public static bool IsInteropString(this string typeName) =>
        typeName == "LPSTR" || typeName == "LPCSTR";
}
