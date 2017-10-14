using System;

namespace Lenic.Web.WebApi.Expressions
{
    public interface IRemoteDataFetcher
    {
        object GetObject(RemoteDataParameter parameter, Type targetType);
    }
}