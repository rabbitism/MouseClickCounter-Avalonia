using System;

namespace MouseClickCounter.Services.Interfaces
{
    public interface ILogService
    {
        void WriteLog(string message);
        void WriteError(string errorMessage, Exception? ex = null);
        void WriteInfo(string infoMessage);
        void WriteDebug(string debugMessage);
        bool LogDirectoryExists();
        string GetTodayLogFilePath();
    }
}
