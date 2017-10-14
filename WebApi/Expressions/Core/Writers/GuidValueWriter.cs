using System;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class GuidValueWriter : ValueWriterBase<Guid>
    {
        public override string Write(object value)
        {
            return string.Format("guid'{0}'", value);
        }
    }
}