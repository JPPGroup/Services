using System;
using System.Threading.Tasks;

namespace Jpp.MessageBroker.Generics
{
    public interface ISendChannel<T> : IDisposable
    {
        Task SendMessageAsync(T message);
    }
}
