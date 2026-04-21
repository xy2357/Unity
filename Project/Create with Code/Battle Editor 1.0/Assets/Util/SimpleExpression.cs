using System;
using System.Collections.Generic;
using System.Globalization;

namespace BattleEditor.Util
{
    /// <summary> Very small expression evaluator for formulas like "Atk*1.2+Mag*0.6" with clamp/min/max and variables. </summary>
    public static class SimpleExpression
    {
        public delegate double VarResolver(string name);

        static readonly HashSet<string> functions = new HashSet<string> { "min", "max", "clamp" };

        public static double Eval(string expr, VarResolver vars)
        {
            var rpn = ToRPN(Tokenize(expr));
            var stack = new Stack<double>();
            foreach (var t in rpn)
            {
                if (t.Type == TokType.Number) stack.Push(t.Number);
                else if (t.Type == TokType.Var) stack.Push(vars(t.Text));
                else if (t.Type == TokType.Op)
                {
                    double b = stack.Pop(), a = stack.Pop();
                    switch (t.Text)
                    {
                        case "+": stack.Push(a + b); break;
                        case "-": stack.Push(a - b); break;
                        case "*": stack.Push(a * b); break;
                        case "/": stack.Push(b == 0 ? 0 : a / b); break;
                    }
                }
                else if (t.Type == TokType.Func)
                {
                    if (t.Text == "min") { double b = stack.Pop(), a = stack.Pop(); stack.Push(Math.Min(a, b)); }
                    else if (t.Text == "max") { double b = stack.Pop(), a = stack.Pop(); stack.Push(Math.Max(a, b)); }
                    else if (t.Text == "clamp") { double c = stack.Pop(); double b = stack.Pop(); double a = stack.Pop(); stack.Push(Math.Max(b, Math.Min(a, c))); }
                }
            }
            return stack.Count > 0 ? stack.Pop() : 0;
        }

        enum TokType { Number, Var, Op, LParen, RParen, Comma, Func }
        class Tok { public TokType Type; public string Text; public double Number; }

        static List<Tok> Tokenize(string s)
        {
            var list = new List<Tok>();
            int i = 0;
            while (i < s.Length)
            {
                char c = s[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }
                if ("+-*/() ,".IndexOf(c) >= 0)
                {
                    list.Add(new Tok { Type = c == '(' ? TokType.LParen : c == ')' ? TokType.RParen : c == ',' ? TokType.Comma : TokType.Op, Text = c.ToString() });
                    i++; continue;
                }
                if (char.IsDigit(c) || c=='.' || (c=='-' && i+1 < s.Length && char.IsDigit(s[i+1])))
                {
                    int start = i; i++;
                    while (i < s.Length && "0123456789.eE+-".IndexOf(s[i]) >= 0) i++;
                    var str = s.Substring(start, i - start);
                    double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double num);
                    list.Add(new Tok { Type = TokType.Number, Number = num, Text = str });
                    continue;
                }
                if (char.IsLetter(c) || c=='_')
                {
                    int start = i; i++;
                    while (i < s.Length && (char.IsLetterOrDigit(s[i]) || s[i]=='_' || s[i]=='.')) i++;
                    var str = s.Substring(start, i - start);
                    if (functions.Contains(str)) list.Add(new Tok { Type = TokType.Func, Text = str });
                    else list.Add(new Tok { Type = TokType.Var, Text = str });
                    continue;
                }
                i++; // skip unknown
            }
            return list;
        }

        static int Prec(string op) => (op == "+" || op == "-") ? 1 : 2;

        static List<Tok> ToRPN(List<Tok> tokens)
        {
            var output = new List<Tok>();
            var ops = new Stack<Tok>();
            foreach (var t in tokens)
            {
                if (t.Type == TokType.Number || t.Type == TokType.Var) output.Add(t);
                else if (t.Type == TokType.Func) ops.Push(t);
                else if (t.Type == TokType.Op)
                {
                    while (ops.Count > 0 && (ops.Peek().Type == TokType.Op && Prec(ops.Peek().Text) >= Prec(t.Text))) output.Add(ops.Pop());
                    ops.Push(t);
                }
                else if (t.Type == TokType.LParen) ops.Push(t);
                else if (t.Type == TokType.RParen)
                {
                    while (ops.Count > 0 && ops.Peek().Type != TokType.LParen) output.Add(ops.Pop());
                    if (ops.Count > 0 && ops.Peek().Type == TokType.LParen) ops.Pop();
                    if (ops.Count > 0 && ops.Peek().Type == TokType.Func) output.Add(ops.Pop());
                }
                else if (t.Type == TokType.Comma)
                {
                    while (ops.Count > 0 && ops.Peek().Type != TokType.LParen) output.Add(ops.Pop());
                }
            }
            while (ops.Count > 0) output.Add(ops.Pop());
            return output;
        }
    }
}
