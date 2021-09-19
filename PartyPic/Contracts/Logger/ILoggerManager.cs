using System;

namespace PartyPic.Contracts.Logger
{
    public interface ILoggerManager
    {
        void LogInformation(string message);
        void LogInformation(string message, Exception ex);
        void LogWarning(string message);
        void LogWarning(string message, Exception ex);
        void LogError(string message);
        void LogError(string message, Exception ex);
    }
}
