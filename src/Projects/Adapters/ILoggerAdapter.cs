using System;

namespace Jpp.Web.Service.Adapters
{
    public interface ILoggerAdapter<T>
    {
        void LogError(Exception ex, string message, params object[] args);
        void LogError(string message);
    }
}
