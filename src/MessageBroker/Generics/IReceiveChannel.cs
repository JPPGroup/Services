using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jpp.MessageBroker.Generics
{
    public interface IReceiveChannel<T> : IDisposable
    {
        Task<T> ReceiveMessageAsync(CancellationToken cancellationToken);

        void RequestComplete();
        void RequestFailed();
    }
}
