using System.Messaging;

namespace Lenic.Framework.Common.Net.MSMQ
{
    internal static class MessageProxyCreater
    {
        public static MessageQueue CreateRemoteQueue(string path)
        {
            return new MessageQueue(path);
        }

        public static MessageQueue CreateLocalQueue(string path)
        {
            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path);

            return new MessageQueue(path);
        }
    }
}