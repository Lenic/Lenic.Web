namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class SingleValueWriter : RationalValueWriter<float>
    {
        protected override string Suffix
        {
            get { return "f"; }
        }
    }
}