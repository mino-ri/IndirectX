using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SyntaxAnalysis;
using Eval = System.Func<System.Collections.Generic.Dictionary<string, string>, long>;

namespace IndirectX.TypeGenerator;

public static class Preprocessor
{
    private static Lexer IfLexer;
    private static PatternParser<Eval> IfParser;
    private static Regex SymbolRegex = new Regex("[a-zA-Z_][a-zA-Z0-9_]*", RegexOptions.Compiled);

    static Preprocessor()
    {
        IfLexer = new Lexer();
        IfLexer.AddIgnore(@"\s");
        var defined = IfLexer.Add("defined", "DEFINED");
        var hex = IfLexer.Add(@"0x[0-9a-fA-F]+", "HEX");
        var dec = IfLexer.Add(@"[+-]?[0-9]+", "DEC");
        var symbol = IfLexer.Add("[a-zA-Z_][a-zA-Z0-9_]*", "SYMBOL");
        var lp = IfLexer.Add(@"\(", "(");
        var rp = IfLexer.Add(@"\)", ")");
        var or = IfLexer.Add(@"\|\|", "||");
        var and = IfLexer.Add(@"\&\&", "&&");
        var cpr = IfLexer.Add(@"[<>!=]=|[<>]", "COMPARER");
        var not = IfLexer.Add(@"\!", "!");

        var expr = Patterns.Choice<Eval>();
        var factor = Patterns.Choice(
                Patterns.Seq(lp, expr, rp, (_, v, _) => v),
                Patterns.Seq(defined, lp, symbol, rp, (_, _, s, _) => (Eval)(syms => syms.ContainsKey(s.Text) ? 1L : 0L)),
                Patterns.Seq(defined, symbol, (_, s) => (Eval)(syms => syms.ContainsKey(s.Text) ? 1L : 0L)),
                dec.Map(t => (Eval)(_ => ParseInt64(t.Text))),
                hex.Map(t => (Eval)(_ => ParseInt64(t.Text))),
                symbol.Map(t => (Eval)(syms => syms.TryGetValue(t.Text, out var s) ? ParseInt64(s!) : 0L)));
        var prefixed = Patterns.Choice(
            factor,
            Patterns.Seq(not, factor, (_, eval) => (Eval)(syms => eval(syms) == 0L ? 1L : 0L)));

        expr.AddRange(
            prefixed,
            Patterns.Seq(expr, cpr, expr, (x, c, y) => (Eval)(syms => c.Text switch
            {
                "<" => x(syms) < y(syms) ? 1L : 0L,
                ">" => x(syms) > y(syms) ? 1L : 0L,
                "<=" => x(syms) <= y(syms) ? 1L : 0L,
                ">=" => x(syms) >= y(syms) ? 1L : 0L,
                "!=" => x(syms) != y(syms) ? 1L : 0L,
                "==" => x(syms) == y(syms) ? 1L : 0L,
                _ => 0L
            }), "compared", Priority.Left(10)),
            Patterns.Seq(expr, and, expr, (x, _, y) => (Eval)(syms => x(syms) != 0 && y(syms) != 0 ? 1L : 0L), "and", Priority.Right(9)),
            Patterns.Seq(expr, or, expr, (x, _, y) => (Eval)(syms => x(syms) != 0 || y(syms) != 0 ? 1L : 0L), "or", Priority.Right(8)));

        IfParser = expr.CreateParser();

        static long ParseInt64(string str)
        {
            str = str.Replace("(", "").Replace(")", "");
            return str.StartsWith("0x") || str.StartsWith("0X")
                ? Convert.ToInt64(str, 16)
                : Convert.ToInt64(str);
        }
    }

    public static (IEnumerable<string>, List<string>) PreProcess(this IEnumerable<string> source, Dictionary<string, string> symbols)
    {
        var includes = new List<string>();
        var ifStack = new Stack<bool?>();
        // true : スキップしない
        // false: スキップする
        // null : スキップする(これまでのifブロックでtrueがあった)
        var l = "";

        return (Core(), includes);

        IEnumerable<string> Core()
        {
            foreach (var line in source)
            {
                l += line;
                if (l.EndsWith('\\'))
                {
                    l = l[..^1];
                    continue;
                }

                if (l.TrimStart().StartsWith('#'))
                {
                    l = l.TrimStart();
                    if (l.StartsWith("#include"))
                    {
                        includes.Add(l["#include".Length..].Trim('"', ' ', '<', '>'));
                    }
                    if (l.StartsWith("#define"))
                    {
                        if (ifStack.All(x => x ?? false))
                        {
                            var m = SymbolRegex.Match(l, "#define".Length);
                            if (m.Success)
                                symbols[m.Value] = l[(m.Index + m.Length)..].Trim();
                        }
                    }
                    else if (l.StartsWith("#undef"))
                    {
                        if (ifStack.All(x => x ?? false))
                        {
                            var m = SymbolRegex.Match(l, "#undef".Length);
                            if (m.Success)
                                symbols.Remove(m.Value);
                        }
                    }
                    else if (l.StartsWith("#ifdef"))
                    {
                        var m = SymbolRegex.Match(l, "#ifdef".Length);
                        ifStack.Push(m.Success && symbols.ContainsKey(m.Value));
                    }
                    else if (l.StartsWith("#ifndef"))
                    {
                        var m = SymbolRegex.Match(l, "#ifndef".Length);
                        ifStack.Push(m.Success && !symbols.ContainsKey(m.Value));
                    }
                    else if (l.StartsWith("#if"))
                    {
                        ifStack.Push(IfParser.Parse(IfLexer.Tokenize(l["#if".Length..]))(symbols) != 0);
                    }
                    else if (l.StartsWith("#elif"))
                    {
                        if (ifStack.Pop() != false)
                        {
                            ifStack.Push(null);
                        }
                        else
                        {
                            ifStack.Push(IfParser.Parse(IfLexer.Tokenize(l["#elif".Length..]))(symbols) != 0);
                        }
                    }
                    else if (l.StartsWith("#else"))
                    {
                        ifStack.Push(ifStack.Pop() == false);
                    }
                    else if (l.StartsWith("#endif"))
                    {
                        ifStack.Pop();
                    }
                }
                else if (ifStack.All(x => x ?? false) && !string.IsNullOrWhiteSpace(l))
                {
                    yield return l;
                }

                l = "";
            }
        }
    }
}
