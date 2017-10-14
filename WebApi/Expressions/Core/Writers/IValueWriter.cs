using System;

namespace Lenic.Web.WebApi.Expressions.Core.Writers
{
    internal interface IValueWriter
    {
        bool Handles(Type type);

        string Write(object value);
    }
}