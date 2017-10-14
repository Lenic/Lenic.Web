namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal abstract class IntegerValueWriter<T> : ValueWriterBase<T>
    {
        public override string Write(object value)
        {
            return value.ToString();
        }
    }
}