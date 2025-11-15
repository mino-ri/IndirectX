using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using IndirectX.TypeGenerator.Templates;

namespace IndirectX.TypeGenerator;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            MainCore(args);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(ex);
            Console.ReadLine();
        }
    }

    static void MainCore(string[] args)
    {
        var spaces = new Regex(@"\s+", RegexOptions.Compiled);

        var inputRoot = "";
        var globalFile = "";
        var settingFiles = new List<string>();
        var outputRoot = Environment.CurrentDirectory;
        var createH = false;
        var createSym = false;
        var createXml = false;
        var wait = false;
        var s = 'l';

        foreach (var arg in args)
        {
            if (arg.StartsWith('-') && arg.Length >= 2)
            {
                if (arg[1] == '?')
                {
                    ShowHelp();
                    return;
                }
                s = arg[1];

                switch (s)
                {
                    case 'x': createXml = true; break;
                    case 'h': createH = true; break;
                    case 's': createSym = true; break;
                    case 'w': wait = true; break;
                }
            }
            else
            {
                switch (s)
                {
                    case 'p': inputRoot = arg; break;
                    case 'g': globalFile = arg; break;
                    case 'l': settingFiles.Add(arg); break;
                }
            }
        }

        if (inputRoot == "")
        {
            ShowHelp();
            return;
        }

        if (settingFiles.Count == 0)
        {
            settingFiles.Add("TypeMapping.xml");
        }

        var symbols = new Dictionary<string, string>();
        var globalSetting = LoadSetting(globalFile, null);
        var settings = settingFiles.Select(s => LoadSetting(s, globalSetting)).ToArray();
        foreach (var setting in settings)
        {
            if (setting.FileName == "")
            {
                foreach (var (k, v) in setting.Symbols) symbols[k] = v;
            }
        }

        foreach (var setting in settings)
        {
            var localSymbols = new Dictionary<string, string>(symbols);
            foreach (var (k, v) in setting.Symbols) localSymbols[k] = v;
            var (lines, icd) = File.ReadLines(Path.Combine(inputRoot, setting.FileName), Encoding.UTF8).PreProcess(localSymbols);
            var code = string.Join(Environment.NewLine, lines);
            if (createH)
                File.WriteAllText(Path.Combine(outputRoot, setting.FileName + ".h"), code, Encoding.UTF8);
            if (createSym)
                File.WriteAllLines(Path.Combine(outputRoot, setting.FileName + ".tsv"), localSymbols.Select(kv => $"{kv.Key}\t{spaces.Replace(kv.Value, " ")}"), Encoding.Default);

            var definitions = CppTypeLoader.Parse(code).ToList();
            if (createXml)
                CreateXml(definitions, setting.Namespace, icd, Path.Combine(outputRoot, setting.FileName + ".xml"));

            foreach (var (enumName, importFrom) in setting.SymbolImports)
            {
                definitions.Add(new EnumDefinition(enumName,
                localSymbols
                    .Where(kv => kv.Key.StartsWith(importFrom))
                    .Select(kv => new EnumFieldDefinition(kv.Key, kv.Value))));
            }

            var enums = definitions.OfType<EnumDefinition>().ToArray();
            if (enums.Any())
            {
                File.WriteAllText(Path.Combine(outputRoot, Path.ChangeExtension(setting.FileName, ".Enums.cs")),
                    new EnumTemplate(enums, setting).TransformText(),
                    Encoding.UTF8);
            }

            var structs = definitions.OfType<StructDefinition>().ToArray();
            if (structs.Any())
            {
                File.WriteAllText(Path.Combine(outputRoot, Path.ChangeExtension(setting.FileName, ".Structs.cs")),
                    new StructTemplate(structs, setting).TransformText(),
                    Encoding.UTF8);
            }

            var apis = definitions.OfType<MethodDefinition>().ToArray();
            if (apis.Any())
            {
                File.WriteAllText(Path.Combine(outputRoot, Path.ChangeExtension(setting.FileName, ".Apis.cs")),
                    new ApiTemplate(apis, setting).TransformText(),
                    Encoding.UTF8);
            }

            foreach (var intf in definitions.OfType<InterfaceDefinition>())
            {
                File.WriteAllText(Path.Combine(outputRoot, setting.MapTypeName(intf.Name) + ".cs"),
                    new InterfaceTemplate(intf, setting).TransformText(),
                    Encoding.UTF8);
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Press enter to exit.");
        Console.ResetColor();

        if (wait)
        {
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }

    static XElement? GetFieldXml(StructMemberDefinition definition)
    {
        return definition switch
        {
            FieldDefinition f => new XElement("Field",
                new XAttribute("Name", f.Name),
                new XAttribute("Type", f.TypeName),
                f.Length.Length == 0 ? null! : new XAttribute("Length", string.Join(',', f.Length))),
            UnionDefinition u => new XElement("Union", u.Fields.Select(GetFieldXml)),
            _ => null,
        };
    }

    static TypeGeneratorSetting LoadSetting(string path, TypeGeneratorSetting? globalSetting)
    {
        if (!File.Exists(path))
            path = Path.Combine(Environment.CurrentDirectory, path);

        return new TypeGeneratorSetting(XElement.Load(path), globalSetting);
    }

    static void CreateXml(IEnumerable<ImportDefinition> definitions, string ns, IEnumerable<string> includes, string outputPath)
    {
        var xml = new XElement("ExportedTypes",
            new XAttribute("Namespace", ns),
            includes.Select(i => new XElement("Include", new XAttribute("Header", i))));
        foreach (var t in definitions)
        {
            xml.Add(t switch
            {
                StructDefinition s => new XElement("Struct",
                    new XAttribute("Name", t.Name),
                    s.Fields.Select(GetFieldXml)),
                EnumDefinition e => new XElement("Enum",
                    new XAttribute("Name", e.Name),
                    e.Fields.Select(f =>
                        new XElement("Case",
                            new XAttribute("Name", f.Name),
                            new XAttribute("Value", f.Value)))),
                InterfaceDefinition i => new XElement("Interface",
                    new XAttribute("Name", i.Name),
                    new XAttribute("Parent", i.ParentName),
                    new XAttribute("Guid", i.Guid),
                    i.Methods.Select(m =>
                        new XElement("Method",
                            new XAttribute("Name", m.Name),
                            new XAttribute("ReturnType", m.ReturnTypeName),
                            m.Parameters.Select(p =>
                                new XElement("Parameter",
                                    new XAttribute("Name", p.Name),
                                    new XAttribute("Type", p.TypeName),
                                    p.Length.Length == 0 ? null! : new XAttribute("Length", string.Join(',', p.Length)),
                                    p.Flags == "" ? null! : new XAttribute("Annotation", p.Flags)))))),
                AliasDefinition a => new XElement("Alias",
                    new XAttribute("Name", a.Name),
                    new XAttribute("Target", a.Target)),
                MethodDefinition m => new XElement("Method",
                            new XAttribute("Name", m.Name),
                            new XAttribute("ReturnType", m.ReturnTypeName),
                            m.Parameters.Select(p =>
                                new XElement("Parameter",
                                    new XAttribute("Name", p.Name),
                                    new XAttribute("Type", p.TypeName),
                                    p.Length.Length == 0 ? null! : new XAttribute("Length", string.Join(',', p.Length)),
                                    p.Flags == "" ? null! : new XAttribute("Annotation", p.Flags)))),
                _ => null,
            });
        }

        xml.Save(outputPath);
    }

    static void ShowHelp()
    {

    }
}
