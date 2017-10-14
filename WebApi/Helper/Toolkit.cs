using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Helper
{
    public static class Toolkit
    {
        private static readonly string LinkTemplate = "<a href=\"{0}\">{1}</a>";
        private static readonly Tuple<char, char> GenericRoundChar = Tuple.Create('<', '>');

        private static ConcurrentDictionary<Assembly, XmlParser> _parserCache = new ConcurrentDictionary<Assembly, XmlParser>();

        public static XmlParser GetParser(Type controllerType)
        {
            if (controllerType == null)
                return null;

            return _parserCache.GetOrAdd(controllerType.Assembly, p =>
            {
                var path = Path.GetDirectoryName(p.CodeBase.Substring(8));
                var name = Path.GetFileNameWithoutExtension(p.Location) + ".xml";
                var fullPathName = Path.Combine(path, name);

                if (File.Exists(fullPathName))
                    return XmlParser.Load(fullPathName);
                else
                    return null;
            });
        }

        public static string GetTypeName(Type type, bool useShortName = true, Tuple<char, char> genericRoundChar = null)
        {
            if (type == null)
                return null;

            if (useShortName && type.IsValueType)
            {
                var newType = Nullable.GetUnderlyingType(type);
                if (newType != null)
                    return string.Format("{0}?", GetTypeName(newType));

                if (type == typeof(byte))
                    return "byte";
                if (type == typeof(sbyte))
                    return "sbyte";
                if (type == typeof(short))
                    return "short";
                if (type == typeof(ushort))
                    return "ushort";
                if (type == typeof(int))
                    return "int";
                if (type == typeof(uint))
                    return "uint";
                if (type == typeof(long))
                    return "long";
                if (type == typeof(ulong))
                    return "ulong";
                if (type == typeof(float))
                    return "float";
                if (type == typeof(decimal))
                    return "decimal";
                if (type == typeof(double))
                    return "double";
                if (type == typeof(DBNull))
                    return "DBNull";
                if (type == typeof(char))
                    return "char";
                if (type == typeof(bool))
                    return "bool";
                if (type == typeof(DateTime))
                    return "DateTime";
            }

            if (useShortName && type == typeof(string))
                return "string";

            if (useShortName && type == typeof(void))
                return "void";

            if (useShortName && type.IsArray)
                return string.Format("{0}[]", GetTypeName(type.GetElementType(), true, genericRoundChar));

            if (!type.IsGenericType)
                return type.FullName;

            var root = type.GetGenericTypeDefinition().FullName;
            root = root.Substring(0, root.Length - 2);
            var parameters = type.GetGenericArguments().Select(p => GetTypeName(p, useShortName, genericRoundChar)).ToArray();

            if (genericRoundChar == null)
                genericRoundChar = GenericRoundChar;

            return string.Format("{0}{2}{1}{3}", root, string.Join(", ", parameters), genericRoundChar.Item1, genericRoundChar.Item2);
        }

        public static TypeURL GetTypeURL<T>()
        {
            return GetTypeURL(typeof(T));
        }

        public static TypeURL GetTypeURL(Type targetType)
        {
            if (targetType == null)
                return TypeURL.Empty;

            var newType = Nullable.GetUnderlyingType(targetType);
            if (newType != null)
                return string.Format("{0}?", GetCLRTypeShortNameString(newType));
            else if (IsCLRType(targetType))
                return GetCLRTypeShortNameString(targetType);

            if (targetType == typeof(void))
                return "void";

            if (targetType.IsArray)
            {
                return new TypeURL
                {
                    HideLink = false,
                    TypeString = "{0}" + Encode("[]"),
                    Metadata = targetType,
                }
                .Setup(p => p.Parameters.Add(GetTypeURL(targetType.GetElementType())));
            }

            if (!targetType.IsGenericType)
            {
                return new TypeURL
                {
                    HideLink = false,
                    TypeString = targetType.FullName,
                    Metadata = targetType,
                };
            }

            var root = targetType.GetGenericTypeDefinition().FullName;
            root = root.Substring(0, root.Length - 2);
            var parameters = targetType.GetGenericArguments().Select(p => GetTypeURL(p)).ToArray();

            return new TypeURL
            {
                HideLink = false,
                TypeString = root + Encode("<") + "{0}" + Encode(">"),
                Metadata = targetType,
            }
            .Setup(p => parameters.ForEach(p.Parameters.Add));
        }

        private static string GetCLRTypeShortNameString(Type targetType)
        {
            if (targetType.IsValueType)
            {
                if (targetType == typeof(byte))
                    return "byte";
                if (targetType == typeof(sbyte))
                    return "sbyte";
                if (targetType == typeof(short))
                    return "short";
                if (targetType == typeof(ushort))
                    return "ushort";
                if (targetType == typeof(int))
                    return "int";
                if (targetType == typeof(uint))
                    return "uint";
                if (targetType == typeof(long))
                    return "long";
                if (targetType == typeof(ulong))
                    return "ulong";
                if (targetType == typeof(float))
                    return "float";
                if (targetType == typeof(decimal))
                    return "decimal";
                if (targetType == typeof(double))
                    return "double";
                if (targetType == typeof(DBNull))
                    return "DBNull";
                if (targetType == typeof(char))
                    return "char";
                if (targetType == typeof(bool))
                    return "bool";
                if (targetType == typeof(DateTime))
                    return "DateTime";

                return targetType.FullName;
            }

            if (targetType == typeof(string))
                return "string";
            else
                return null;
        }

        private static string Encode(string source)
        {
            return HttpContext.Current.Server.HtmlEncode(source);
        }

        public static bool IsFrameworkType(Type type)
        {
            return type.IsArray || type.FullName.StartsWith("System") || type.FullName.StartsWith("Microsoft");
        }

        public static bool IsCLRType(Type targetType)
        {
            var newType = Nullable.GetUnderlyingType(targetType);
            if (newType != null)
                targetType = newType;

            if (targetType.IsValueType)
            {
                if (targetType == typeof(byte))
                    return true;
                if (targetType == typeof(sbyte))
                    return true;
                if (targetType == typeof(short))
                    return true;
                if (targetType == typeof(ushort))
                    return true;
                if (targetType == typeof(int))
                    return true;
                if (targetType == typeof(uint))
                    return true;
                if (targetType == typeof(long))
                    return true;
                if (targetType == typeof(ulong))
                    return true;
                if (targetType == typeof(float))
                    return true;
                if (targetType == typeof(decimal))
                    return true;
                if (targetType == typeof(double))
                    return true;
                if (targetType == typeof(DBNull))
                    return true;
                if (targetType == typeof(char))
                    return true;
                if (targetType == typeof(bool))
                    return true;
                if (targetType == typeof(DateTime))
                    return true;

                return false;
            }

            if (targetType == typeof(string))
                return true;
            else
                return false;
        }

        public static string GetTypeNameString(Type type)
        {
            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }

        public static R IfNull<T, R>(this T obj, R trueObject, Func<T, R> falseAction)
        {
            return ReferenceEquals(obj, null) ? trueObject : falseAction(obj);
        }

        public static string GetHtmlString(TypeURL obj)
        {
            if (ReferenceEquals(obj, TypeURL.Empty))
                return string.Empty;

            if (obj.HideLink)
                return obj.ToString();

            if (obj.Parameters.Any())
                return string.Format(obj.TypeString, string.Join(", ", obj.Parameters.Select(p => GetHtmlString(p))));

            var url = string.Format("{0}://{1}/Help/Classes.aspx?typeFullName={2}",
                HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Server.UrlEncode(GetTypeNameString(obj.Metadata)));
            return string.Format(LinkTemplate, url, obj.TypeString);
        }
    }

    public class TypeURL
    {
        public static readonly TypeURL Empty = new TypeURL();

        public bool HideLink { get; set; }

        public string TypeString { get; set; }

        public Type Metadata { get; set; }

        public ICollection<TypeURL> Parameters { get; set; }

        public TypeURL()
        {
            Parameters = new Collection<TypeURL>();
        }

        public override string ToString()
        {
            if (Parameters.Any())
                return string.Format(TypeString, string.Join(", ", Parameters.Select(p => p.ToString()).ToArray()));
            else
                return TypeString;
        }

        public static implicit operator TypeURL(string obj)
        {
            return new TypeURL
            {
                HideLink = true,
                TypeString = obj,
            };
        }
    }
}