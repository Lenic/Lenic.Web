using Lenic.Web.WebApi.Services;

namespace Lenic.Web.WebApi.ExtensionPoints
{
    internal class GlobalObjectProcessorX : GlobalObjectProcessor
    {
        public static GlobalObjectProcessorX Instance = new GlobalObjectProcessorX();

        public GlobalObjectProcessorX()
            : base()
        {
        }
    }
}