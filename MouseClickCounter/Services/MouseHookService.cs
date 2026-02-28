using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MouseClickCounter.Services.Interfaces;

namespace MouseClickCounter.Services
{
    /// <summary>
    /// 鼠标点击监控服务 - 跨平台实现
    /// </summary>
    public class MouseHookService : IMouseHookService
    {
        private readonly ILogService _logService;
        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelMouseProc? _proc;
        private bool _isHooked = false;

        // 计数变量
        private long _leftClickCount = 0;
        private long _rightClickCount = 0;

        // Windows API声明
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        public MouseHookService(ILogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 安装鼠标钩子
        /// </summary>
        public bool InstallHook()
        {
            if (_isHooked)
                return true;

            try
            {
                // 只在Windows上安装钩子
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    _proc = HookCallback;
                    using (Process curProcess = Process.GetCurrentProcess())
                    using (ProcessModule? curModule = curProcess.MainModule)
                    {
                        if (curModule != null)
                        {
                            _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc,
                                GetModuleHandle(curModule.ModuleName), 0);

                            if (_hookID == IntPtr.Zero)
                            {
                                return false;
                            }
                        }
                    }
                    _isHooked = true;
                    return true;
                }
                else
                {
                    // 在非Windows平台上，我们不能使用钩子
                    // 这里可以考虑使用其他方法，如事件监听
                    _logService.WriteInfo("非Windows平台，鼠标钩子功能不可用");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logService.WriteError("安装鼠标钩子失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 卸载鼠标钩子
        /// </summary>
        public void UninstallHook()
        {
            if (_isHooked && _hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
                _isHooked = false;
            }
        }

        /// <summary>
        /// 鼠标钩子回调
        /// </summary>
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    if (wParam == (IntPtr)WM_LBUTTONDOWN)
                    {
                        System.Threading.Interlocked.Increment(ref _leftClickCount);
                    }
                    else if (wParam == (IntPtr)WM_RBUTTONDOWN)
                    {
                        System.Threading.Interlocked.Increment(ref _rightClickCount);
                    }
                }
            }
            catch
            {
                // 忽略所有异常，确保鼠标消息能正常传递
            }

            // 必须调用 CallNextHookEx 来传递消息
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        /// <summary>
        /// 获取左键点击次数
        /// </summary>
        public long GetLeftClickCount()
        {
            return System.Threading.Interlocked.Read(ref _leftClickCount);
        }

        /// <summary>
        /// 获取右键点击次数
        /// </summary>
        public long GetRightClickCount()
        {
            return System.Threading.Interlocked.Read(ref _rightClickCount);
        }

        /// <summary>
        /// 设置左键点击次数
        /// </summary>
        public void SetLeftClickCount(long count)
        {
            System.Threading.Interlocked.Exchange(ref _leftClickCount, count);
        }

        /// <summary>
        /// 设置右键点击次数
        /// </summary>
        public void SetRightClickCount(long count)
        {
            System.Threading.Interlocked.Exchange(ref _rightClickCount, count);
        }

        /// <summary>
        /// 重置计数
        /// </summary>
        public void ResetCounts()
        {
            System.Threading.Interlocked.Exchange(ref _leftClickCount, 0);
            System.Threading.Interlocked.Exchange(ref _rightClickCount, 0);
        }

        public void Dispose()
        {
            UninstallHook();
        }
    }
}
