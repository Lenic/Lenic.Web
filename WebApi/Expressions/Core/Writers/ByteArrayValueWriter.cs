using System;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal class ByteArrayValueWriter : ValueWriterBase<byte[]>
    {
        public override string Write(object value)
        {
            var base64 = Convert.ToBase64String((byte[])value);
            return string.Format("X'{0}'", base64);
        }
    }
}