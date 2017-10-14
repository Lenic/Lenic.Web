using System;

namespace Lenic.Framework.Common.Expressions
{
    internal interface IElementTypeProvider
    {
        Type OriginalElementType { get; set; }
    }
}