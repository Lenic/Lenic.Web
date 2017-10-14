using System;
using System.Xml;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class DateTimeValueWriter : ValueWriterBase<DateTime>
    {
        public override string Write(object value)
        {
            var dateTimeValue = (DateTime)value;

            return string.Format("datetime'{0}'", XmlConvert.ToString(dateTimeValue, XmlDateTimeSerializationMode.Utc));
        }
    }
}