using System;
using System.IO;
using System.Threading.Tasks;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services;

/// <summary>
/// 日志服务
/// </summary>
public class LogService : ILogService
{
    private readonly string _logDirectory;

    /// <summary>
    /// 创建日志服务实例
    /// </summary>
    public LogService()
    {
        _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
    }

    /// <summary>
    /// 写入日志
    /// </summary>
    public async Task WriteLogAsync(string message)
    {
        try
        {
            string logPath = _logDirectory;
            if (!Directory.Exists(logPath))
            {
                try
                {
                    Directory.CreateDirectory(logPath);
                }
                catch
                {
                    // 如果创建目录失败，尝试写入临时目录
                    logPath = Path.GetTempPath();
                }
            }

            string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
            string fullPath = Path.Combine(logPath, fileName);
            await File.AppendAllTextAsync(fullPath, $"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        }
        catch
        {
            // 如果所有日志写入都失败，忽略错误
        }
    }

    /// <summary>
    /// 写入错误日志
    /// </summary>
    public async Task WriteErrorAsync(string errorMessage, Exception? ex = null)
    {
        string message = $"错误: {errorMessage}";
        if (ex != null)
        {
            message += $"\r\n异常详情: {ex.Message}\r\n堆栈跟踪: {ex.StackTrace}";
        }
        await WriteLogAsync(message);
    }

    /// <summary>
    /// 写入信息日志
    /// </summary>
    public async Task WriteInfoAsync(string infoMessage)
    {
        await WriteLogAsync($"信息: {infoMessage}");
    }

    /// <summary>
    /// 写入调试日志
    /// </summary>
    public async Task WriteDebugAsync(string debugMessage)
    {
        await WriteLogAsync($"调试: {debugMessage}");
    }

    /// <summary>
    /// 检查日志目录是否存在
    /// </summary>
    public bool LogDirectoryExists()
    {
        return Directory.Exists(_logDirectory);
    }

    /// <summary>
    /// 获取今天的日志文件路径
    /// </summary>
    public string GetTodayLogFilePath()
    {
        string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
        return Path.Combine(_logDirectory, fileName);
    }
}