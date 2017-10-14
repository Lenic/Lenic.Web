namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class DoubleValueWriter : RationalValueWriter<double>
    {
        protected override string Suffix
        {
            get { return string.Empty; }
        }
    }
}