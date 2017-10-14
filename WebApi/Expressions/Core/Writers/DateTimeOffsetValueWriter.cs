using System;
using System.Xml;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class DateTimeOffsetValueWriter : ValueWriterBase<DateTimeOffset>
    {
        public override string Write(object value)
        {
            return string.Format("datetimeoffset'{0}'", XmlConvert.ToString((DateTimeOffset)value));
        }
    }
}