using System.Globalization;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal abstract class RationalValueWriter<T> : ValueWriterBase<T>
    {
        protected abstract string Suffix { get; }

        public override string Write(object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", value, Suffix);
        }
    }
}