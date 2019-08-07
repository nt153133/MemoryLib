using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace MemLib.Ffxiv.XivApi {
    // Repo: https://github.com/zanders3/json
    // Really simple JSON parser in ~300 lines
    // - Attempts to parse JSON files with minimal GC allocation
    // - Nice and simple "[1,2,3]".FromJson<List<int>>() API
    // - Classes and structs can be parsed too!
    //      class Foo { public int Value; }
    //      "{\"Value\":10}".FromJson<Foo>()
    // - Can parse JSON without type information into Dictionary<string,object> and List<object> e.g.
    //      "[1,2,3]".FromJson<object>().GetType() == typeof(List<object>)
    //      "{\"Value\":10}".FromJson<object>().GetType() == typeof(Dictionary<string,object>)
    // - No JIT Emit support to support AOT compilation on iOS
    // - Attempts are made to NOT throw an exception if the JSON is corrupted or invalid: returns null instead.
    // - Only public fields and property setters on classes/structs will be written to
    //
    // Limitations:
    // - No JIT Emit support to parse structures quickly
    // - Limited to parsing <2GB JSON files (due to int.MaxValue)
    // - Parsing of abstract classes or interfaces is NOT supported and will throw an exception.
    internal static class JsonParser {
        [ThreadStatic] private static Stack<List<string>> m_SplitArrayPool;
        [ThreadStatic] private static StringBuilder m_StringBuilder;
        [ThreadStatic] private static Dictionary<Type, Dictionary<string, FieldInfo>> m_FieldInfoCache;
        [ThreadStatic] private static Dictionary<Type, Dictionary<string, PropertyInfo>> m_PropertyInfoCache;

        public static T FromJson<T>(this string json) {
            // Initialize, if needed, the ThreadStatic variables
            if (m_PropertyInfoCache == null) m_PropertyInfoCache = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
            if (m_FieldInfoCache == null) m_FieldInfoCache = new Dictionary<Type, Dictionary<string, FieldInfo>>();
            if (m_StringBuilder == null) m_StringBuilder = new StringBuilder();
            if (m_SplitArrayPool == null) m_SplitArrayPool = new Stack<List<string>>();

            //Remove all whitespace not within strings to make parsing simpler
            m_StringBuilder.Length = 0;
            for (var i = 0; i < json.Length; i++) {
                var c = json[i];
                if (c == '"') {
                    i = AppendUntilStringEnd(true, i, json);
                    continue;
                }

                if (char.IsWhiteSpace(c))
                    continue;

                m_StringBuilder.Append(c);
            }

            //Parse the thing!
            return (T) ParseValue(typeof(T), m_StringBuilder.ToString());
        }

        private static int AppendUntilStringEnd(bool appendEscapeCharacter, int startIdx, string json) {
            m_StringBuilder.Append(json[startIdx]);
            for (var i = startIdx + 1; i < json.Length; i++)
                if (json[i] == '\\') {
                    if (appendEscapeCharacter)
                        m_StringBuilder.Append(json[i]);
                    m_StringBuilder.Append(json[i + 1]);
                    i++; //Skip next character as it is escaped
                }
                else if (json[i] == '"') {
                    m_StringBuilder.Append(json[i]);
                    return i;
                }
                else {
                    m_StringBuilder.Append(json[i]);
                }

            return json.Length - 1;
        }

        //Splits { <value>:<value>, <value>:<value> } and [ <value>, <value> ] into a list of <value> strings
        private static List<string> Split(string json) {
            var splitArray = m_SplitArrayPool.Count > 0 ? m_SplitArrayPool.Pop() : new List<string>();
            splitArray.Clear();
            if (json.Length == 2)
                return splitArray;
            var parseDepth = 0;
            m_StringBuilder.Length = 0;
            for (var i = 1; i < json.Length - 1; i++) {
                switch (json[i]) {
                    case '[':
                    case '{':
                        parseDepth++;
                        break;
                    case ']':
                    case '}':
                        parseDepth--;
                        break;
                    case '"':
                        i = AppendUntilStringEnd(true, i, json);
                        continue;
                    case ',':
                    case ':':
                        if (parseDepth == 0) {
                            splitArray.Add(m_StringBuilder.ToString());
                            m_StringBuilder.Length = 0;
                            continue;
                        }

                        break;
                }

                m_StringBuilder.Append(json[i]);
            }

            splitArray.Add(m_StringBuilder.ToString());

            return splitArray;
        }

        internal static object ParseValue(Type type, string json) {
            if (type == typeof(string)) {
                if (json.Length <= 2)
                    return string.Empty;
                var parseStringBuilder = new StringBuilder(json.Length);
                for (var i = 1; i < json.Length - 1; ++i) {
                    if (json[i] == '\\' && i + 1 < json.Length - 1) {
                        var j = "\"\\nrtbf/".IndexOf(json[i + 1]);
                        if (j >= 0) {
                            parseStringBuilder.Append("\"\\\n\r\t\b\f/"[j]);
                            ++i;
                            continue;
                        }

                        if (json[i + 1] == 'u' && i + 5 < json.Length - 1) {
                            if (uint.TryParse(json.Substring(i + 2, 4), NumberStyles.AllowHexSpecifier, null, out var c)) {
                                parseStringBuilder.Append((char) c);
                                i += 5;
                                continue;
                            }
                        }
                    }

                    parseStringBuilder.Append(json[i]);
                }

                return parseStringBuilder.ToString();
            }

            if (type.IsPrimitive) {
                var result = Convert.ChangeType(json, type, CultureInfo.InvariantCulture);
                return result;
            }

            if (type == typeof(decimal)) {
                decimal.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                return result;
            }

            if (json == "null") return null;
            if (type.IsEnum) {
                if (json[0] == '"')
                    json = json.Substring(1, json.Length - 2);
                try {
                    return Enum.Parse(type, json, false);
                }
                catch {
                    return 0;
                }
            }

            if (type.IsArray) {
                var arrayType = type.GetElementType();
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                var elems = Split(json);
                var newArray = Array.CreateInstance(arrayType ?? throw new InvalidOperationException(), elems.Count);
                for (var i = 0; i < elems.Count; i++)
                    newArray.SetValue(ParseValue(arrayType, elems[i]), i);
                m_SplitArrayPool.Push(elems);
                return newArray;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)) {
                var listType = type.GetGenericArguments()[0];
                if (json[0] != '[' || json[json.Length - 1] != ']')
                    return null;

                var elems = Split(json);
                var list = (IList) type.GetConstructor(new[] {typeof(int)})?.Invoke(new object[] {elems.Count});
                for (var i = 0; i < elems.Count; i++) {
                    list?.Add(ParseValue(listType, elems[i]));
                }

                m_SplitArrayPool.Push(elems);
                return list;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                Type keyType, valueType;
                {
                    var args = type.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }

                //Refuse to parse dictionary keys that aren't of type string
                if (keyType != typeof(string))
                    return null;
                //Must be a valid dictionary element
                if (json[0] != '{' || json[json.Length - 1] != '}')
                    return null;
                //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
                var elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;

                var dictionary =
                    (IDictionary) type.GetConstructor(new[] {typeof(int)})?.Invoke(new object[] {elems.Count / 2});
                for (var i = 0; i < elems.Count; i += 2) {
                    if (elems[i].Length <= 2)
                        continue;
                    var keyValue = elems[i].Substring(1, elems[i].Length - 2);
                    var val = ParseValue(valueType, elems[i + 1]);
                    dictionary?.Add(keyValue, val);
                }

                return dictionary;
            }

            if (type == typeof(object)) return ParseAnonymousValue(json);
            if (json[0] == '{' && json[json.Length - 1] == '}') return ParseObject(type, json);

            return null;
        }

        private static object ParseAnonymousValue(string json) {
            if (json.Length == 0)
                return null;
            if (json[0] == '{' && json[json.Length - 1] == '}') {
                var elems = Split(json);
                if (elems.Count % 2 != 0)
                    return null;
                var dict = new Dictionary<string, object>(elems.Count / 2);
                for (var i = 0; i < elems.Count; i += 2)
                    dict.Add(elems[i].Substring(1, elems[i].Length - 2), ParseAnonymousValue(elems[i + 1]));
                return dict;
            }

            if (json[0] == '[' && json[json.Length - 1] == ']') {
                var items = Split(json);
                var finalList = new List<object>(items.Count);
                for (var i = 0; i < items.Count; i++)
                    finalList.Add(ParseAnonymousValue(items[i]));
                return finalList;
            }

            if (json[0] == '"' && json[json.Length - 1] == '"') {
                var str = json.Substring(1, json.Length - 2);
                return str.Replace("\\", string.Empty);
            }

            if (char.IsDigit(json[0]) || json[0] == '-') {
                if (json.Contains(".")) {
                    double.TryParse(json, NumberStyles.Float, CultureInfo.InvariantCulture, out var result);
                    return result;
                }
                else {
                    int.TryParse(json, out var result);
                    return result;
                }
            }

            if (json == "true")
                return true;
            if (json == "false")
                return false;
            // handles json == "null" as well as invalid JSON
            return null;
        }

        private static Dictionary<string, T> CreateMemberNameDictionary<T>(T[] members) where T : MemberInfo {
            var nameToMember = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < members.Length; i++) {
                var member = members[i];
                if (member.IsDefined(typeof(IgnoreDataMemberAttribute), true))
                    continue;

                var name = member.Name;
                if (member.IsDefined(typeof(DataMemberAttribute), true)) {
                    var dataMemberAttribute =
                        (DataMemberAttribute) Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        name = dataMemberAttribute.Name;
                }

                nameToMember.Add(name, member);
            }

            return nameToMember;
        }

        private static object ParseObject(Type type, string json) {
            var instance = FormatterServices.GetUninitializedObject(type);

            //The list is split into key/value pairs only, this means the split must be divisible by 2 to be valid JSON
            var elems = Split(json);
            if (elems.Count % 2 != 0)
                return instance;

            if (!m_FieldInfoCache.TryGetValue(type, out var nameToField)) {
                nameToField = CreateMemberNameDictionary(
                    type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                m_FieldInfoCache.Add(type, nameToField);
            }

            if (!m_PropertyInfoCache.TryGetValue(type, out var nameToProperty)) {
                nameToProperty = CreateMemberNameDictionary(
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
                m_PropertyInfoCache.Add(type, nameToProperty);
            }

            for (var i = 0; i < elems.Count; i += 2) {
                if (elems[i].Length <= 2)
                    continue;
                var key = elems[i].Substring(1, elems[i].Length - 2);
                var value = elems[i + 1];

                if (nameToField.TryGetValue(key, out var fieldInfo))
                    fieldInfo.SetValue(instance, ParseValue(fieldInfo.FieldType, value));
                else if (nameToProperty.TryGetValue(key, out var propertyInfo))
                    propertyInfo.SetValue(instance, ParseValue(propertyInfo.PropertyType, value), null);
            }

            return instance;
        }
    }
}