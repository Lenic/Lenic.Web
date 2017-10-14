namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class DecimalValueWriter : RationalValueWriter<decimal>
    {
        protected override string Suffix
        {
            get { return "m"; }
        }
    }
}