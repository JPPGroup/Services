using System;
using System.Threading.Tasks;

namespace Jpp.MessageBroker
{
    public interface IMessageChanel<T> : IDisposable
    {
        Task SendMessageAsync(T message);
        Task<T> ReceiveMessageAsync();
        void RequestComplete();
        void RequestFailed();
    }
}
