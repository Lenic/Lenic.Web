using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lenic.Framework.Common.Extensions;
using Lenic.Framework.Common.IO;

namespace Helper.Help
{
    public partial class Classes : System.Web.UI.Page
    {
        public string TypeFullName { get; set; }

        public ClassMetaData DataSource { get; set; }

        public Type TargetType { get; set; }

        public XmlParser Parser { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TypeFullName = Server.UrlDecode(Request["typeFullName"]);

            TargetType = Type.GetType(TypeFullName);
            Parser = Toolkit.GetParser(TargetType);

            LoadData(TargetType);
        }

        private void LoadData(Type type)
        {
            PropertyMetaData[] data = null;
            if (type.BaseType != typeof(Enum))
                data = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new PropertyMetaData
                {
                    Name = p.Name,
                    Type = Toolkit.GetTypeName(p.PropertyType),
                    OriginalTypeIdentity = GetTypeInfoURL(p.PropertyType),
                    IsCLRType = Toolkit.IsFrameworkType(p.PropertyType),
                    Description = GetPropertyDescription(p),
                })
                .ToArray();
            else
                data = type.GetFields().Skip(1).Select(p => new PropertyMetaData
                {
                    Name = p.Name,
                    Type = Toolkit.GetTypeName(p.FieldType),
                    OriginalTypeIdentity = GetTypeInfoURL(p.FieldType),
                    IsCLRType = Toolkit.IsFrameworkType(p.FieldType),
                    Description = GetFieldDescription(p),
                })
                .ToArray();

            var obj = new ClassMetaData
            {
                Name = TargetType.Name,
                Description = GetTypeDescription(),
                Properties = data,
            };

            DataSource = obj;
        }

        private string GetTypeDescription()
        {
            var defaultTypeDescription = "--";

            if (Parser == null)
                return defaultTypeDescription;

            var name = string.Format("T:{0}", TargetType.FullName);
            var node = Parser.InnerParsers
                             .First(p => p.Type == "members")
                             .InnerParsers
                             .FirstOrDefault(p => p.GetString("name") == name);

            if (node == null)
                return defaultTypeDescription;

            return node.InnerParsers.FirstOrDefault(p => p.Type == "summary").IfNull(defaultTypeDescription, p => p.Text);
        }

        private string GetPropertyDescription(PropertyInfo property)
        {
            return GetMemberDescription(property, string.Format("P:{0}.{1}", property.DeclaringType.FullName, property.Name));
        }

        private string GetFieldDescription(FieldInfo field)
        {
            return GetMemberDescription(field, string.Format("F:{0}.{1}", field.DeclaringType.FullName, field.Name));
        }

        private string GetMemberDescription(MemberInfo member, string name)
        {
            var defaultMemberDescription = "--";

            if (Parser == null)
                return defaultMemberDescription;

            //var name = string.Format("P:{0}.{1}", member.DeclaringType.FullName, member.Name);
            var node = Parser.InnerParsers
                             .First(p => p.Type == "members")
                             .InnerParsers
                             .FirstOrDefault(p => p.GetString("name") == name);

            if (node == null)
                return defaultMemberDescription;

            return node.InnerParsers.FirstOrDefault(p => p.Type == "summary").IfNull(defaultMemberDescription, p => p.Text).Trim();
        }

        private string GetTypeInfoURL(Type type)
        {
            return string.Format("{0}://{1}/Help/Classes.aspx?typeFullName={2}", Request.Url.Scheme, Request.Url.Authority, Server.UrlEncode(Toolkit.GetTypeNameString(type)));
        }

        public class ClassMetaData
        {
            public string Name { get; set; }

            public bool IsCLRType { get; set; }

            public string Description { get; set; }

            public PropertyMetaData[] Properties { get; set; }
        }

        public class PropertyMetaData
        {
            public string Name { get; set; }

            public string Type { get; set; }

            public string OriginalTypeIdentity { get; set; }

            public bool IsCLRType { get; set; }

            public string Description { get; set; }
        }
    }
}