using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IndirectX.TypeGenerator;

public class TypeGeneratorSetting
{
    private static readonly Regex Head = new Regex("(?<![a-z])[a-z]", RegexOptions.Compiled);
    private static readonly Regex Ps = new Regex(@"^p+[A-Z]|^[A-Z]", RegexOptions.Compiled);

    private static int[] ParseIntArray(string? str)
    {
        if (string.IsNullOrEmpty(str)) return Array.Empty<int>();
        return str.Split(',').Select(int.Parse).ToArray();
    }

    public XElement Root { get; }

    public (Regex, string)[] NameReplaces { get; }

    public Dictionary<string, string> Symbols { get; }

    public (string name, string from)[] SymbolImports { get; }

    public string FileName { get; }

    public string DllName { get; }

    public string Namespace { get; }

    public string[] ImportNamespaces { get; }

    public TypeMapping[] TypeMappings { get; }

    private string GetRefText(ParameterDefinition paramDef, ImportDefinition[] hierarchy)
    {
        return IsIn(paramDef, hierarchy) ? "in"
             : IsOut(paramDef, hierarchy) ? "out"
                                          : "ref";
    }

    public string MapNativeTypeName(ParameterDefinition paramDef, ImportDefinition[] hierarchy)
    {
        var nativeType = paramDef.TypeName.IsInteropString()
            ? "InteropString"
            : paramDef.TypeName.IsInterfacePointer()
            ? "ComPtr" + new string('*', paramDef.TypeName.Count(c => c == '*') - 1)
            : MapTypeName(paramDef.TypeName, paramDef.Length);

        if (!nativeType.EndsWith('*')) return nativeType;

        if (nativeType == "void*") return "void*";
        if (nativeType == "void**") nativeType = "ComPtr*";
        return IsArray(paramDef, hierarchy)
            ? nativeType
            : $"{GetRefText(paramDef, hierarchy)} {nativeType[..^1]}";
    }

    private bool IsIn(ParameterDefinition p, ImportDefinition[] hierarchy) => (bool?)Get("In", hierarchy) ?? p.IsIn;

    private bool IsOut(ParameterDefinition p, ImportDefinition[] hierarchy) => (bool?)Get("Out", hierarchy) ?? p.IsOut;

    private bool IsArray(ParameterDefinition p, ImportDefinition[] hierarchy) => (bool?)Get("ByArray", hierarchy) ?? p.IsArray;

    private bool IsOptional(ParameterDefinition p, ImportDefinition[] hierarchy) => (bool?)Get("Optional", hierarchy) ?? p.IsOptional;

    public ManagedParameterUsage MapManagedUsage(ParameterDefinition paramDef, MethodDefinition methodDef, InterfaceDefinition? intDef)
    {
        var hierarchy = CrateHierarchy(intDef, methodDef, paramDef);
        if (GetString("Use", hierarchy) is { } use)
            return new ManagedParameterUsage("", "", use, "", "", "", null, ParameterDirection.None);

        var isStr = paramDef.TypeName.IsInteropString();
        var comPtr = paramDef.TypeName.IsInterfacePointer();
        var name = MapParameterName(paramDef, hierarchy);
        var type = GetString("ManagedType", hierarchy) ??
            (comPtr ? MapTypeName(paramDef.TypeName[..^1], paramDef.Length)
                    : MapTypeName(paramDef.TypeName, paramDef.Length));

        var isIn = IsIn(paramDef, hierarchy);
        var isOut = IsOut(paramDef, hierarchy);
        var usage = (string?)null;
        var @return = GetString("Return", hierarchy);
        var isOptional = IsOptional(paramDef, hierarchy);

        if (!GetBool("NotLength", hierarchy) && methodDef.Parameters.FirstOrDefault(p =>
            IsIn(p, CrateHierarchy(intDef, methodDef, p)) && IsArray(p, CrateHierarchy(intDef, methodDef, p)) &&
            p.ReferenceParameter == paramDef.Name) is { } c)
        {
            var arrayHierarchy = CrateHierarchy(intDef, methodDef, c);
            var arrayName = MapParameterName(c, arrayHierarchy);
            var optionalArgText = IsOptional(c, arrayHierarchy) ? "0" : null;
            return new ManagedParameterUsage("", "", $"{arrayName}.Length", @return ?? name, "", "", optionalArgText, ParameterDirection.None);
        }

        if (isStr)
        {
            if (isOut && methodDef.Parameters.FirstOrDefault(p => paramDef.ReferenceParameter == p.Name) is { } c1)
            {
                var countName = GetString("Length", hierarchy) ?? MapParameterName(c1, CrateHierarchy(intDef, methodDef, c1));
                @return ??= $"{name}.ToString()";
                return new ManagedParameterUsage("string", $"string {name}", name, @return, "",
                    $"using (var {name} = new InteropString({countName}))", null, ParameterDirection.Output);
            }
            else if (isIn)
            {
                return new ManagedParameterUsage("string", $"string {name}", $"p_{name}", "", "",
                    $"using (var p_{name} = new InteropString({name}))", null, ParameterDirection.Input);
            }

            type = "byte*";
        }

        // not pointer
        if (!type.EndsWith('*') || type == "void*")
        {
            @return ??= name;
            if (!comPtr)
            {
                usage ??= name;
            }
            else if (isOptional)
            {
                type = type + "?";
                usage ??= $"{name}?.ComPtr ?? default";
            }
            else
            {
                usage ??= $"{name}.ComPtr";
            }

            return new ManagedParameterUsage(type, $"{type} {name}", usage, @return, "", "", isOptional ? "default" : null, ParameterDirection.Input);
        }
        else
        {
            var refText = GetRefText(paramDef, hierarchy) + " ";
            type = type == "void**" ? "ComPtr" : type[..^1];

            if (IsArray(paramDef, hierarchy))
            {
                if (isOut && methodDef.Parameters.FirstOrDefault(p => paramDef.ReferenceParameter == p.Name) is { } c1)
                {
                    var countName = GetString("Length", hierarchy) ?? MapParameterName(c1, CrateHierarchy(intDef, methodDef, c1));
                    var prestatement = comPtr
                        ? ""
                        : $"var {name} = new {type}[{countName}];";
                    var blockStatement = comPtr
                        ? $"using (var {name} = new InteropArray<{type}>({countName}))"
                        : $"fixed ({type}* p_{name} = {name})";
                    usage ??= comPtr ? $"{name}.NativePtr" : $"p_{name}";
                    @return ??= comPtr ? $"{name}.ToArray()" : name;
                    return new ManagedParameterUsage($"{type}[]", $"{type}[] {name}", usage, @return, prestatement, blockStatement, null, ParameterDirection.Output);
                }
                // out 型でも引数を上書きするパターンはこちら
                else
                {
                    var paramsText = GetBool("Params", hierarchy) ? "params " : "";
                    var blockStatement = comPtr
                        ? $"using (var p_{name} = new InteropArray<{type}>({name}))"
                        : $"fixed ({type}* p_{name} = {name})";
                    type = comPtr || paramsText != "" ? $"{type}[]"
                         : isIn ? $"ReadOnlySpan<{type}>"
                                                      : $"Span<{type}>";
                    usage ??= comPtr ? $"p_{name}.NativePtr" : $"p_{name}";
                    var optionalArgText = isOptional && isIn ? "default" : null;
                    return new ManagedParameterUsage(type, $"{paramsText}{type} {name}", usage, "", "", blockStatement, optionalArgText, ParameterDirection.Input);
                }
            }

            if (isOut)
            {
                usage ??= $"out var {name}";
                @return ??= comPtr ? $"new {type}({name})" : name;
                return new ManagedParameterUsage(type, $"{type} {name}", usage, @return, "", "", null, ParameterDirection.Output);
            }
            else
            {
                usage ??= comPtr ? $"{name}.ComPtr" : $"{refText}{name}";
                var optionalArgText = !isOptional || !isIn ? null : !comPtr
                    ? $"in Unsafe.NullRef<{type}>()"
                    : "in default";
                return new ManagedParameterUsage(type, $"{refText}{type} {name}", usage, "", "", "", optionalArgText, ParameterDirection.Input);
            }
        }
    }

    private static ImportDefinition[] CrateHierarchy(params ImportDefinition?[] source)
    {
        return source.Where(x => x is not null).ToArray()!;
    }

    public string MapParameterName(string memberName)
    {
        return Ps.Replace(memberName, m => m.Value[^1].ToString().ToLower());
    }

    public string MapParameterName(ParameterDefinition paramDef, ImportDefinition[] hierarchy)
    {
        if ((string?)Get("MapTo", hierarchy) is { } mapTo) return mapTo;
        return MapParameterName(paramDef.Name);
    }

    public string MapTypeName(string type, params int[] length)
    {
        if (length.Any())
        {
            foreach (var mapping in TypeMappings)
            {
                if (type == mapping.Name && length.SequenceEqual(mapping.Length))
                {
                    return mapping.Target;
                }
            }

            return $"{ToPascalCase(type)}{string.Join('x', length)}";
        }
        else
        {
            var unpointeredType = type.TrimEnd('*');
            foreach (var mapping in TypeMappings)
            {
                if (unpointeredType == mapping.Name && mapping.Length.Length == 0)
                {
                    return mapping.Target + type[unpointeredType.Length..];
                }
            }
        }

        var result = ToPascalCase(type);
        return result.EndsWith("Flag") ? result + "s" : result;
    }

    public string ToPascalCase(string name)
    {
        name = name.Contains('_') || !name.Any(c => 'a' <= c && c <= 'z')
            ? Head.Replace(name.ToLowerInvariant(), c => c.Value.ToUpperInvariant()).Replace("_", "")
            : name;

        name = name[0..1].ToUpper() + name[1..];

        foreach (var (regex, val) in NameReplaces)
        {
            name = regex.Replace(name, val);
        }

        return name;
    }

    public XAttribute? Get(string name, params ImportDefinition[] defHierarchy)
    {
        return GetCore(Root, 0);

        XAttribute? GetCore(XElement target, int index)
        {
            if (index >= defHierarchy.Length) return null;

            var def = defHierarchy[index];
            if (target.Elements(def.ElementName)
                      .FirstOrDefault(e => (string?)e.Attribute("Name") == def.Name) is { } t)
            {
                if (index == defHierarchy.Length - 1)
                {
                    return t.Attribute(name);
                }
                else
                {
                    var result = GetCore(t, index + 1);
                    if (result is not null)
                        return result;
                }
            }

            if (target.Element("Common") is { } common &&
                common.Elements(defHierarchy.Last().ElementName)
                      .FirstOrDefault(e => Utilities.IsMatchAll((string?)e.Attribute("Name"), defHierarchy.Last().Name)) is { } t1 &&
                      t1.Attribute(name) is { } result1)
            {
                return result1;
            }

            return null;
        }
    }

    public string? GetString(string name, params ImportDefinition[] defHierarchy) =>
        (string?)Get(name, defHierarchy);

    public int GetInt(string name, params ImportDefinition[] defHierarchy) =>
        (int?)Get(name, defHierarchy) ?? 0;

    public bool GetBool(string name, params ImportDefinition[] defHierarchy) =>
        (bool?)Get(name, defHierarchy) ?? false;

    public bool IsCommentOut(params ImportDefinition[] defHierarchy) =>
        GetBool(nameof(IsCommentOut), defHierarchy);

    public TypeGeneratorSetting(XElement root, TypeGeneratorSetting? globalSetting)
    {
        if (globalSetting is { })
            root.AddFirst(globalSetting.Root.Elements());

        Root = root;
        Namespace = (string?)root.Attribute("Namespace") ?? "Missing";
        FileName = (string?)root.Attribute("Include") ?? "";
        DllName = (string?)root.Attribute("Dll") ?? FileName.Replace(".h", ".dll");
        ImportNamespaces =
            root.Elements("Import").Attributes("Namespace")
            .Select(a => (string?)a)
            .Where(a => a is not null)
            .ToArray()!;
        TypeMappings = Root.Elements("Alias")
            .Select(e => new TypeMapping(
                (string?)e.Attribute("Name") ?? "",
                (string?)e.Attribute("MapTo") ?? "",
                ParseIntArray((string?)e.Attribute("Length"))))
            .ToArray();

        Symbols = new Dictionary<string, string>();
        foreach (var define in root.Elements("Define"))
        {
            if ((string?)define.Attribute("Symbol") is { } symbol)
            {
                Symbols[symbol] = (string?)define.Attribute("Value") ?? "";
            }
        }

        SymbolImports = root.Elements("Enum")
            .SelectMany(e => e.Attributes("FromSymbol"),
            (e, a) => ((string?)e.Attribute("Name") ?? (string?)a ?? "", (string?)a ?? ""))
            .ToArray();

        NameReplaces = root.Elements("NameReplace")
            .Select(r => (new Regex((string?)r.Attribute("From") ?? ""), (string?)r.Attribute("To") ?? ""))
            .ToArray();
    }
}

public record TypeMapping(string Name, string Target, int[] Length);

public record ManagedParameterUsage(string Type, string ParameterText, string ArgText, string ReturnText, string Prestatement, string BlockStatement, string? OptionalArgText, ParameterDirection Direction)
{
    public bool IsInput => Direction == ParameterDirection.Input;
    public bool IsOutput => Direction == ParameterDirection.Output;
}

public enum ParameterDirection
{
    None,
    Input,
    Output,
}
