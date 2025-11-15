using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace IndirectX.TypeGenerator;

public record ExportModel(TypeGeneratorSetting Setting, params ImportDefinition[] Hierarchy)
{
    public string SourceName => Hierarchy.Last().Name;

    public string Modifier => GetString(nameof(Modifier)) ?? "public";

    public string? MapTo => GetString(nameof(MapTo));

    public bool Delete => GetBool(nameof(Delete));

    public XAttribute? Get(string name) => Setting.Get(name, Hierarchy);

    public string? GetString(string name) => (string?)Get(name);

    public bool GetBool(string name) => (bool?)Get(name) ?? false;
}

public record EnumModel(TypeGeneratorSetting Setting, EnumDefinition EnumDef)
    : ExportModel(Setting, EnumDef)
{
    public CaseModel[] Cases { get; } = EnumDef.Fields.Select(f => new CaseModel(Setting, EnumDef, f)).ToArray();

    public bool IsFlag => Name.EndsWith("Flags");

    public bool RequiredToMakeFlag => IsFlag && !Cases.Any(f => f.Name.ToLower() == "none");

    public string Name => MapTo ?? Setting.MapTypeName(EnumDef.Name);

    public string BaseType => GetString(nameof(BaseType)) ?? "uint";
}

public record CaseModel(TypeGeneratorSetting Setting, EnumDefinition EnumDef, EnumFieldDefinition CaseDef)
    : ExportModel(Setting, EnumDef, CaseDef)
{
    private static readonly Regex Id = new Regex(@"\b[a-zA-Z][a-zA-Z0-9_]*\b", RegexOptions.Compiled);
    private static readonly Regex HexL = new Regex(@"\b0x[0-9a-fA-F]+[uU]?[lL]\b", RegexOptions.Compiled);
    private static readonly Regex DecL = new Regex(@"\b[0-9]+[uU]?[lL]\b", RegexOptions.Compiled);

    public string Name => MapTo ?? MapEnumField(EnumDef.Name, CaseDef.Name);

    public string Value => MapEnumValue(EnumDef.Name, CaseDef.Value);

    private string MapEnumField(string enumName, string fieldName)
    {
        var lastBar = 0;
        var index = 0;
        foreach (var (x, y) in fieldName.Zip(enumName + "_", (x, y) => (x, y)))
        {
            index++;
            if (x == '_') lastBar = index;
            if (x != y) break;
        }

        return Setting.ToPascalCase(fieldName[lastBar..]);
    }

    private string MapEnumValue(string enumName, string value)
    {
        return value.Replace(Id, m => MapEnumField(enumName, m.Value))
                    .Replace(HexL, m => m.Value.TrimEnd('u', 'U', 'l', 'L'))
                    .Replace(DecL, m => m.Value.TrimEnd('u', 'U', 'l', 'L'));
    }
}

public record StructModel(TypeGeneratorSetting Setting, StructDefinition StructDef)
    : ExportModel(Setting, StructDef)
{
    public ExportModel[] Members { get; } = StructDef.Fields.Select(m => m switch
        {
            FieldDefinition f => new FieldModel(Setting, new[] { StructDef }, f),
            UnionDefinition u => new UnionModel(Setting, new[] { StructDef }, u),
            _ => (ExportModel)null!,
        })
        .ToArray();

    public string Name => MapTo ?? Setting.MapTypeName(StructDef.Name);

    public bool CanGenerateConstructor => !Members.OfType<UnionModel>().Any();

    public string ParameterText => Members
        .OfType<FieldModel>()
        .Select(f => $"{f.Type} {f.ParameterName}")
        .StringJoin(", ");
}

public record FieldModel(TypeGeneratorSetting Setting, ImportDefinition[] ParentDefs, FieldDefinition FieldDef)
    : ExportModel(Setting, ParentDefs.Append(FieldDef).ToArray())
{
    public string Type => GetString("Type") ?? Setting.MapTypeName(FieldDef.TypeName, FieldDef.Length);

    public string Name => MapTo ?? Setting.ToPascalCase(FieldDef.Name);

    public string ParameterName => Setting.MapParameterName(Name);
}

public record UnionModel(TypeGeneratorSetting Setting, ImportDefinition[] ParentDefs, UnionDefinition UnionDef)
    : ExportModel(Setting, ParentDefs.Append(UnionDef).ToArray())
{
    public ExportModel[] Members { get; } = UnionDef.Fields.Select(m => m switch
        {
            FieldDefinition f => new FieldModel(Setting, ParentDefs.Append(UnionDef).ToArray(), f),
            UnionDefinition u => new UnionModel(Setting, ParentDefs.Append(UnionDef).ToArray(), u),
            _ => (ExportModel)null!,
        })
        .ToArray();
}

public record InterfaceModel(TypeGeneratorSetting Setting, InterfaceDefinition InterfaceDef)
    : ExportModel(Setting, InterfaceDef)
{
    public MethodModel[] Methods { get; } = InterfaceDef.Methods.Select(m => new MethodModel(Setting, InterfaceDef, m)).ToArray();

    public string Name => MapTo ?? Setting.MapTypeName(InterfaceDef.Name);

    public string ParentName => Setting.MapTypeName(InterfaceDef.ParentName);

    public Guid Guid => InterfaceDef.Guid;

    public (string name, string type, bool hasSetter)[] Properties
    {
        get
        {
            var getters = Methods.Where(m => m.Name.StartsWith("Get") &&
                                             m.Modifier == "public" &&
                                             !m.Inputs.Any() &&
                                             (m.ReturnNative || m.Outputs.Count() == 1))
                                 .Select(m => (name: m.Name[3..], m.ManagedReturnType));
            var setters = Methods.Where(m => m.Name.StartsWith("Set") &&
                                             m.Modifier == "public" &&
                                             m.Inputs.Count() == 1 &&
                                             !m.Outputs.Any())
                                 .Select(m => (name: m.Name[3..], m.Inputs.Single().ManagedType));
            return (from getter in getters
                    join setter in setters
                    on getter equals setter into ss
                    select (getter.name, getter.ManagedReturnType, ss.Any()))
                    .ToArray();
        }
    }
}

public record MethodModel(TypeGeneratorSetting Setting, InterfaceDefinition? InterfaceDef, MethodDefinition MethodDef)
    : ExportModel(Setting, InterfaceDef is null
        ? new ImportDefinition[] { MethodDef }
        : new ImportDefinition[] { InterfaceDef, MethodDef })
{
    public ParameterModel[] Parameters { get; } = MethodDef.Parameters.Select(p => new ParameterModel(Setting, InterfaceDef, MethodDef, p)).ToArray();

    public IEnumerable<ParameterModel> Inputs => Parameters.Where(p => p.IsInput);

    public IEnumerable<ParameterModel> Outputs => Parameters.Where(p => p.IsOutput);

    public bool ReturnNative => (bool?)Get(nameof(ReturnNative)) ??
        (MethodDef.ReturnTypeName != "void" && MethodDef.ReturnTypeName != "HRESULT");

    public string NativeName => MethodDef.Name;

    public string Name => MapTo ?? MethodDef.Name;

    public string ApiName => MapTo ?? Setting.ToPascalCase(MethodDef.Name);

    public string NativeReturnType => Setting.MapTypeName(MethodDef.ReturnTypeName);

    public string ManagedReturnType => ReturnNative
        ? NativeReturnType
        : Outputs.Count() switch
        {
            0 => "void",
            1 => Outputs.First().ManagedType,
            _ => $"({Outputs.Select(u => u.ParameterText).StringJoin(", ")})",
        };

    public string NativeParameterText => Parameters.Select(p => $"{p.NativeType} {p.NativeName}").StringJoin(", ");

    public string ParameterText => Inputs.Select(u => u.ParameterText).StringJoin(", ");

    public string OptionalParameterText => Inputs.Where(u => !u.IsOptional).Select(u => u.ParameterText).StringJoin(", ");

    public string ArgText => Parameters.Select(u => ", " + u.ArgText).StringJoin("");

    public string OptionalArgText => Parameters.Select(u => ", " + u.OptionalArgText).StringJoin("");

    public string ReturnText => Outputs.Select(u => u.ReturnText).StringJoin(", ");

    public IEnumerable<string> Prestatements => Parameters
        .Select(p => p.Prestatement)
        .Where(s => !string.IsNullOrWhiteSpace(s));

    public IEnumerable<string> BlockStatements => Parameters
        .Select(p => p.BlockStatement)
        .Where(s => !string.IsNullOrWhiteSpace(s));

    public IEnumerable<string> OptionalPrestatements => Parameters
        .Where(p => !p.IsOptional && !string.IsNullOrWhiteSpace(p.Prestatement))
        .Select(p => p.Prestatement);

    public IEnumerable<string> OptionalBlockStatements => Parameters
        .Where(p => !p.IsOptional && !string.IsNullOrWhiteSpace(p.BlockStatement))
        .Select(p => p.BlockStatement);
}

public record ParameterModel(TypeGeneratorSetting Setting, InterfaceDefinition? InterfaceDef, MethodDefinition MethodDef, ParameterDefinition ParameterDef)
    : ExportModel(Setting, InterfaceDef is null
        ? new ImportDefinition[] { MethodDef, ParameterDef }
        : new ImportDefinition[] { InterfaceDef, MethodDef, ParameterDef })
{
    public ManagedParameterUsage Usage { get; } = Setting.MapManagedUsage(ParameterDef, MethodDef, InterfaceDef);

    public string Name => MapTo ?? ParameterDef.Name;

    public bool IsInput => Usage.IsInput;

    public bool IsOutput => Usage.IsOutput;

    public bool IsOptional => Usage.OptionalArgText is not null;

    public string ManagedType => Usage.Type;

    public string NativeName => ParameterDef.Name;

    public string NativeType => GetString("NativeType") ?? Setting.MapNativeTypeName(ParameterDef, Hierarchy);

    public string ParameterText => Usage.ParameterText;

    public string ArgText => Usage.ArgText;

    public string OptionalArgText => Usage.OptionalArgText ?? Usage.ArgText;

    public string ReturnText => Usage.ReturnText;

    public string Prestatement => Usage.Prestatement;

    public string BlockStatement => Usage.BlockStatement;
}
