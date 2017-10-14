using System;
using System.Xml;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class TimeSpanValueWriter : ValueWriterBase<TimeSpan>
    {
        public override string Write(object value)
        {
            return string.Format("time'{0}'", XmlConvert.ToString((TimeSpan)value));
        }
    }
}