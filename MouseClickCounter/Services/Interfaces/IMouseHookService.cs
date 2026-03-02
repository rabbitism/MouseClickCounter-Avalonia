using System;

namespace MouseClickCounter.Services.Interfaces;

public interface IMouseHookService : IDisposable
{
    bool InstallHook();
    void UninstallHook();
    long GetLeftClickCount();
    long GetRightClickCount();
    void SetLeftClickCount(long count);
    void SetRightClickCount(long count);
    void ResetCounts();
}