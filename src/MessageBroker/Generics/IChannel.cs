using System;
using System.Threading.Tasks;
using Jpp.MessageBroker.Generics;

namespace Jpp.MessageBroker
{
    public interface IChannel<T> : ISendChannel<T>, IReceiveChannel<T>
    {
    }
}
