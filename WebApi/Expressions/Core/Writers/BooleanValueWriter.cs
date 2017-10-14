namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class BooleanValueWriter : ValueWriterBase<bool>
    {
        public override string Write(object value)
        {
            var boolean = (bool)value;

            return boolean ? "true" : "false";
        }
    }
}