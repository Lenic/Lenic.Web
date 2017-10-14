using System;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal abstract class ValueWriterBase<T> : IValueWriter
    {
        public bool Handles(Type type)
        {
            return typeof(T) == type;
        }

        public abstract string Write(object value);
    }
}