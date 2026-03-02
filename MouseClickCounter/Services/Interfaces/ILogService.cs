using System;
using System.Threading.Tasks;

namespace MouseClickCounter.Services.Interfaces;

public interface ILogService
{
    Task WriteLogAsync(string message);
    Task WriteErrorAsync(string errorMessage, Exception? ex = null);
    Task WriteInfoAsync(string infoMessage);
    Task WriteDebugAsync(string debugMessage);
    bool LogDirectoryExists();
    string GetTodayLogFilePath();
}