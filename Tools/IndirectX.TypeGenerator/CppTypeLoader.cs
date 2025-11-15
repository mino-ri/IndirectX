using System;
using System.Collections.Generic;
using System.Linq;
using SyntaxAnalysis;

namespace IndirectX.TypeGenerator;

public static class CppTypeLoader
{
    static readonly PatternParser<ImportDefinition> Parser;
    static readonly Func<string, IEnumerable<Token[]>> _tokenize;

    static CppTypeLoader()
    {
        var lexer = new Lexer();
        lexer.AddIgnore(@"\s");
        lexer.AddIgnore(@"/\*.*?\*/");
        lexer.AddIgnore(@"//[^\n]*");
        lexer.AddIgnore(@"//");
        var hex = lexer.Add(@"0x[0-9a-fA-F]+[lL]?", "HEX");
        var num = lexer.Add(@"0|[1-9][0-9]*", "NUM");
        var id = lexer.Add(@"[a-zA-Z_][a-zA-Z0-9_]*", "ID");
        var lb = lexer.Add(@"\{", "{");
        var rb = lexer.Add(@"\}", "}");
        var lp = lexer.Add(@"\(", "(");
        var rp = lexer.Add(@"\)", ")");
        var lr = lexer.Add(@"\[", "[");
        var rr = lexer.Add(@"\]", "]");
        var text = lexer.Add(@"\"".*?\""", "TEXT");
        var comma = lexer.Add(@",", ",");
        var colon = lexer.Add(@":", ":");
        var semicolon = lexer.Add(@";", ";");
        var dot = lexer.Add(@"[.]", ".");
        var eq = lexer.Add(@"[=]", "=");
        var star = lexer.Add(@"\*", "*");
        var op = lexer.Add(@"->|\||\+|\-|>>|<<|[!<>]=", "OP");

        var typedef = new TokenType("'typedef'");
        var @interface = new TokenType("'interface'");
        var @struct = new TokenType("'struct'");
        var @enum = new TokenType("'enum'");
        var union = new TokenType("'union'");
        var @virtual = new TokenType("'virtual'");
        var @public = new TokenType("'public'");
        var midlInterface = new TokenType("MIDL_INTERFACE");
        var @void = new TokenType("'void'");
        var stdcall = new TokenType("STDMETHODCALLTYPE");
        var winapi = new TokenType("'WINAPI'");
        var pFlag = new TokenType("__flag");

        _tokenize = TokenizeCore;

        IEnumerable<Token[]> TokenizeCore(string str)
        {
            using (var e = lexer.Tokenize(str).GetEnumerator())
            {
                var tokens = new List<Token>();
                while (e.MoveNext())
                {
                    Token prev = default;
                    tokens.Clear();
                    while (e.Current.Text != "typedef" &&
                           e.Current.Text != "MIDL_INTERFACE" &&
                           e.Current.Text != "WINAPI")
                    {
                        prev = e.Current;
                        if (!e.MoveNext()) yield break;
                    }

                    if (e.Current.Text == "WINAPI")
                        tokens.Add(prev);

                    tokens.Add(e.Current.With(e.Current.Text switch
                    {
                        "typedef" => typedef,
                        "MIDL_INTERFACE" => midlInterface,
                        "WINAPI" => winapi,
                        _ => throw new Exception(),
                    }));

                    var bracesCount = 0;

                    while (e.MoveNext())
                    {
                        var t = e.Current;

                        if (t.Type == id)
                        {
                            var text = t.Text;
                            switch (text)
                            {
                                case "struct": tokens.Add(t.With(@struct)); break;
                                case "interface": tokens.Add(t.With(@interface)); break;
                                case "enum": tokens.Add(t.With(@enum)); break;
                                case "virtual": tokens.Add(t.With(@virtual)); break;
                                case "public": tokens.Add(t.With(@public)); break;
                                case "void": tokens.Add(t.With(@void)); break;
                                case "union": tokens.Add(t.With(union)); break;
                                case "STDMETHODCALLTYPE": tokens.Add(t.With(stdcall)); break;
                                case "WINAPI": tokens.Add(t.With(winapi)); break;
                                case "const":
                                case "CONST": break;
                                default:
                                    tokens.Add(text.StartsWith("__") ? t.With(pFlag) : t); break;
                            }
                        }
                        else
                        {
                            tokens.Add(t);
                        }

                        if (t.Type == lb) bracesCount++;
                        else if (t.Type == rb) bracesCount--;
                        else if (bracesCount <= 0 && t.Type == semicolon) break;
                    }

                    tokens.Add(new Token(str, tokens.Last().EndIndex, 0, TokenType.Eof));
                    yield return tokens.ToArray();
                }
            }
        }

        var type = Patterns.Choice<string>("type", type => new[]
        {
            @void.Map(_ => "void", "type"),
            id.Map(t => t.Text, "type"),
            Patterns.Seq(type, star, (t, _) => t + "*", "type", Priority.Left(100)),
        });

        // struct
        var field = Patterns.Choice<StructMemberDefinition>("field");
        var fields = Patterns.Seq(lb, Patterns.ListZero(field, "fields", Priority.Right(90)), rb, (_, fs, _) => fs, "fields");
        var index = Patterns.Seq(lr, num, rr, (_, ix, _) => int.Parse(ix.Text), "index");
        var indices = Patterns.ListZero(index, "indices", Priority.Right(80));
        field.AddRange(
            Patterns.Seq(type, id, indices, semicolon, (t, name, ix, _) => new FieldDefinition(name.Text, t, ix.ToArray()), "field"),
            Patterns.Seq(union, fields, semicolon, (_, fields, _) => new UnionDefinition(fields)));
        var structDef = Patterns.Seq(typedef, @struct, id, fields, id, semicolon,
            (_, _, _, fs, name, _) => new StructDefinition(name.Text, fs));

        // enum
        var caseValue = Patterns.ListOne(Patterns.Choice("value",
            id, num, hex, op, lp, rp), "value", Priority.Right(95));
        var @case = Patterns.Choice("case",
            Patterns.Seq(id, eq, caseValue, (name, _, v) => new EnumFieldDefinition(name.Text, string.Join(' ', v.Select(x => x.Text))), "case"),
            id.Map(name => new EnumFieldDefinition(name.Text, ""), "case"));

        var cases = new ChoicePattern<(IEnumerable<EnumFieldDefinition> fields, string name)>()
        {
            Patterns.Seq(@case, rb, id, semicolon, (c, _, name, _) => (Enumerable.Repeat(c, 1), name.Text), "cases", Priority.Right(90)),
            Patterns.Seq(@case, comma, rb, id, semicolon, (c, _, _, name, _) => (Enumerable.Repeat(c, 1), name.Text), "cases", Priority.Right(90)),
        };
        cases.Add(Patterns.Seq(@case, comma, cases, (c, _, cs) => (cs.fields.Prepend(c), cs.name), "cases", Priority.Right(90)));

        var enumDef = Patterns.Seq(typedef, @enum, id, lb, cases, (_, _, _, _, cs) => new EnumDefinition(cs.name, cs.fields));

        // interface
        var optValue = Patterns.ListOne(Patterns.Choice("optValue",
            id, num, hex, op, star, comma), "optValue", Priority.Right(95));
        var parameter = Patterns.Choice(
            Patterns.Seq(pFlag, type, id, indices, (flags, t, name, ix) => new ParameterDefinition(name.Text, t, flags.Text, ix.ToArray()), "parameter"),
            Patterns.Seq(pFlag, lp, optValue, rp, type, id, indices, (flags, _, v, _, t, name, ix) => new ParameterDefinition(name.Text, t, $"{flags.Text}({string.Join(' ', v.Select(x => x.Text))})", ix.ToArray()), "parameter"),
            Patterns.Seq(type, id, indices, (t, name, ix) => new ParameterDefinition(name.Text, t, "", ix.ToArray()), "parameter"));
        var parameters = Patterns.ListOne(parameter, comma, "parameters", Priority.Right(90));
        var methodHeader = Patterns.Seq(@virtual, type, stdcall, id, (_, t, _, name) => (name: name.Text, t), "methodHeader");
        var method = Patterns.Choice(
            Patterns.Seq(methodHeader, lp, @void, rp, eq, num, semicolon, (h, _, _, _, _, _, _) => new MethodDefinition(h.name, h.t, Array.Empty<ParameterDefinition>())),
            Patterns.Seq(methodHeader, lp, parameters, rp, eq, num, semicolon, (h, _, ps, _, _, _, _) => new MethodDefinition(h.name, h.t, ps)));
        var methods = Patterns.ListZero(method, "methods", Priority.Right(80));
        var ifHeader = Patterns.Seq(midlInterface, lp, text, rp, id, colon, @public, id,
            (_, _, guid, _, id, _, _, parent) => (name: id.Text, parent: parent.Text, guid: new Guid(guid.Text.Trim('"'))), "ifHeader");
        var ifDef = Patterns.Seq(ifHeader, lb, @public, colon, methods, rb, semicolon,
            (h, _, _, _, ms, _, _) => new InterfaceDefinition(h.name, h.parent, h.guid, ms));

        // winapi
        var apiHeader = Patterns.Seq(type, winapi, id, lp, parameters, rp, semicolon, (t, _, name, _, ps, _, _) => new MethodDefinition(name.Text, t, ps));

        // alias
        var aliasDef = Patterns.Seq(
            typedef, type, id, semicolon, (_, target, name, _) => new AliasDefinition(target, name.Text));

        // alias(interface)
        var ignoredDef = Patterns.Seq(
            typedef, @interface, type, id, semicolon, (_, _, _, _, _) => IgnoredDefinition.Instance);

        Parser = Patterns.Choice<ImportDefinition>("exportedTypes", structDef, enumDef, ifDef, apiHeader, aliasDef, ignoredDef).CreateParser();
    }

    public static IEnumerable<string> Tokenize(string code)
    {
        return _tokenize(code).Select(x => string.Join(' ', x.Select(t => t.Text)));
    }

    public static IEnumerable<ImportDefinition> Parse(string code)
    {
        foreach (var tokens in _tokenize(code))
        {
            ImportDefinition? definition;
            try
            {
                definition = Parser.Parse(tokens);
            }
            catch (ParseException ex)
            {
                Console.WriteLine(string.Join(' ', tokens.Select(t => t.Text)));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                definition = null;
            }

            if (definition is not null)
            {
                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine(definition.Name);
                //Console.ResetColor();
                yield return definition;
            }
        }
    }
}
