namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class StringValueWriter : ValueWriterBase<string>
    {
        public override string Write(object value)
        {
            return string.Format("'{0}'", value);
        }
    }
}