// Tiny permissive JSON parser that returns Dictionary<string, object> / List<object> / double / string / bool / null.
// Not a full JSON serializer; intended for runtime data loading in this sample.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BattleEditor.Util
{
    public static class FastJson
    {
        public static object Parse(string json)
        {
            int i = 0;
            return ParseValue(json, ref i);
        }

        static void SkipWs(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
        }

        static object ParseValue(string s, ref int i)
        {
            SkipWs(s, ref i);
            if (i >= s.Length) return null;
            char c = s[i];
            if (c == '{') return ParseObject(s, ref i);
            if (c == '[') return ParseArray(s, ref i);
            if (c == '"') return ParseString(s, ref i);
            if (c == 't') { i += 4; return true; }
            if (c == 'f') { i += 5; return false; }
            if (c == 'n') { i += 4; return null; }
            return ParseNumber(s, ref i);
        }

        static Dictionary<string, object> ParseObject(string s, ref int i)
        {
            var dict = new Dictionary<string, object>();
            i++; // {
            while (true)
            {
                SkipWs(s, ref i);
                if (i >= s.Length) break;
                if (s[i] == '}') { i++; break; }
                var key = ParseString(s, ref i);
                SkipWs(s, ref i);
                if (s[i] == ':') i++;
                var val = ParseValue(s, ref i);
                dict[key] = val;
                SkipWs(s, ref i);
                if (s[i] == ',') { i++; continue; }
                if (s[i] == '}') { i++; break; }
            }
            return dict;
        }

        static List<object> ParseArray(string s, ref int i)
        {
            var list = new List<object>();
            i++; // [
            while (true)
            {
                SkipWs(s, ref i);
                if (i >= s.Length) break;
                if (s[i] == ']') { i++; break; }
                var v = ParseValue(s, ref i);
                list.Add(v);
                SkipWs(s, ref i);
                if (s[i] == ',') { i++; continue; }
                if (s[i] == ']') { i++; break; }
            }
            return list;
        }

        static string ParseString(string s, ref int i)
        {
            var sb = new StringBuilder();
            i++; // "
            while (i < s.Length)
            {
                char c = s[i++];
                if (c == '"') break;
                if (c == '\\' && i < s.Length)
                {
                    char e = s[i++];
                    if (e == '"' || e == '\\' || e == '/') sb.Append(e);
                    else if (e == 'b') sb.Append('\b');
                    else if (e == 'f') sb.Append('\f');
                    else if (e == 'n') sb.Append('\n');
                    else if (e == 'r') sb.Append('\r');
                    else if (e == 't') sb.Append('\t');
                    else if (e == 'u')
                    {
                        string hex = s.Substring(i, 4);
                        sb.Append((char)Convert.ToInt32(hex, 16));
                        i += 4;
                    }
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        static double ParseNumber(string s, ref int i)
        {
            int start = i;
            while (i < s.Length && "-+0123456789.eE".IndexOf(s[i]) >= 0) i++;
            var str = s.Substring(start, i - start);
            double.TryParse(str, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double d);
            return d;
        }

        public static string GetString(Dictionary<string, object> d, string key, string def = "")
            => d.ContainsKey(key) ? d[key] as string ?? def : def;
        public static double GetNumber(Dictionary<string, object> d, string key, double def = 0)
            => d.ContainsKey(key) ? (d[key] is double ? (double)d[key] : def) : def;
        public static bool GetBool(Dictionary<string, object> d, string key, bool def = false)
            => d.ContainsKey(key) ? (d[key] is bool ? (bool)d[key] : def) : def;
        public static Dictionary<string, object> GetDict(Dictionary<string, object> d, string key)
            => d.ContainsKey(key) ? d[key] as Dictionary<string, object> : null;
        public static List<object> GetList(Dictionary<string, object> d, string key)
            => d.ContainsKey(key) ? d[key] as List<object> : null;
    }
}
