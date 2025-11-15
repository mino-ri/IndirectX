using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IndirectX.TypeGenerator;

public abstract class ImportDefinition
{
    public abstract string ElementName { get; }

    public string Name { get; set; } = "";
}

public class IgnoredDefinition : ImportDefinition
{
    public override string ElementName => "";

    public static readonly IgnoredDefinition Instance = new IgnoredDefinition();
}

public class AliasDefinition : ImportDefinition
{
    public override string ElementName => "Alias";

    public string Target { get; set; } = "";

    public AliasDefinition(string target, string name) => (Target, Name) = (target, name);
}

public class ParameterDefinition : ImportDefinition
{
    private static readonly Regex ReferenceParameterRegex =
        new Regex(@"[a-zA-Z0-9_]+(?=\))", RegexOptions.Compiled);

    public override string ElementName => "Parameter";

    public string Flags { get; set; } = "";
    public string TypeName { get; set; } = "";
    public int[] Length { get; set; } = Array.Empty<int>();

    public ParameterDefinition(string name, string typeName, string flags, int[] length)
    {
        Name = name;
        Flags = flags;
        TypeName = typeName;
        Length = length;
    }

    public bool IsIn => Flags.StartsWith("__in") && !Flags.StartsWith("__inout");

    public bool IsOut => Flags.StartsWith("__out");

    public bool IsArray => Flags.Contains("_ecount");

    public bool IsOptional => Flags.Contains("_opt");

    public string ReferenceParameter =>
        ReferenceParameterRegex.Match(Flags) is { Success: true } m ? m.Value : "";
}

public class MethodDefinition : ImportDefinition
{
    public override string ElementName => "Method";

    public string ReturnTypeName { get; set; } = "";
    public ParameterDefinition[] Parameters { get; set; } = Array.Empty<ParameterDefinition>();

    public MethodDefinition(string name, string returnTypeName, IEnumerable<ParameterDefinition> parameters)
    {
        Name = name;
        ReturnTypeName = returnTypeName;
        Parameters = parameters.ToArray();
    }
}

public class InterfaceDefinition : ImportDefinition
{
    public override string ElementName => "Interface";

    public Guid Guid { get; set; }
    public string ParentName { get; set; } = "";
    public MethodDefinition[] Methods { get; } = Array.Empty<MethodDefinition>();

    public InterfaceDefinition(string name, string parentName, Guid guid, IEnumerable<MethodDefinition> methods)
    {
        Name = name;
        ParentName = parentName;
        Guid = guid;
        Methods = methods.ToArray();
    }
}

public abstract class StructMemberDefinition : ImportDefinition { }

public class FieldDefinition : StructMemberDefinition
{
    public override string ElementName => "Field";

    public string TypeName { get; set; } = "";
    public int[] Length { get; set; } = Array.Empty<int>();

    public FieldDefinition(string name, string typeName, int[] length)
    {
        Name = name;
        TypeName = typeName;
        Length = length;
    }
}

public class UnionDefinition : StructMemberDefinition
{
    public override string ElementName => "Union";

    public StructMemberDefinition[] Fields { get; set; }

    public UnionDefinition() => Fields = Array.Empty<StructMemberDefinition>();
    public UnionDefinition(IEnumerable<StructMemberDefinition> members) => Fields = members.ToArray();
}

public class StructDefinition : ImportDefinition
{
    public override string ElementName => "Struct";

    public StructMemberDefinition[] Fields { get; set; }

    public StructDefinition(string name, IEnumerable<StructMemberDefinition> members)
    {
        Name = name;
        Fields = members.ToArray();
    }
}

public class EnumFieldDefinition : ImportDefinition
{
    public override string ElementName => "Case";

    public string Value { get; set; } = "";

    public EnumFieldDefinition(string name, string value) => (Name, Value) = (name, value);
}

public class EnumDefinition : ImportDefinition
{
    public override string ElementName => "Enum";

    public EnumFieldDefinition[] Fields { get; set; }

    public EnumDefinition(string name, IEnumerable<EnumFieldDefinition> members)
    {
        Name = name;
        Fields = members.ToArray();
    }
}

[Flags]
public enum ParameterAnnotations
{
    None = 0,
    In = 0x1,
    Out = 0x2,
    Array = 0x4,
    Optional = 0x8,
}
