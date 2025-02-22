﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using static insoLyrics.Interop.NativeMethods;

namespace insoLyrics.Interop
{
    internal static class Osu
    {
        #region Process

        private static Process _process;

        public static Process Process
        {
            get
            {
                if (_process != null)
                {
                    while (_process.MainWindowHandle == IntPtr.Zero)
                    {
                        Thread.Sleep(1000);
                    }
                    return _process;
                }

                // 确保 osu! 正在运行并以 _process 连接
                _process = Process.GetProcessesByName("osu!").FirstOrDefault();
                if (_process != null)
                {
                    return Process;
                }

                // 由于 osu! 没有运行，弹窗警告
                if (Registry.GetValue(@"HKEY_CLASSES_ROOT\osu!\shell\open\command", null, null) is string exec)
                {
                    _process = Process.Start(exec.Split('"')[1]);
                    return Process;
                }

                MessageBox.Show("osu!安装了吗？");
                Application.Exit();
                return null;
            }
        }

        #endregion

        #region Show()

        public static bool IsForeground => GetForegroundWindow() == Process.MainWindowHandle;

        public static void Show()
        {
            ShowWindow(Process.MainWindowHandle, SW_SHOWNORMAL);
            SetForegroundWindow(Process.MainWindowHandle);
        }

        #endregion

        #region HookKeyboard(Action<Keys> action), UnhookKeyboard()

        public static event EventHandler<KeyEventArgs> KeyDown;

        private static IntPtr _hhkk = IntPtr.Zero; // hookHandleKeyKeyboard
        private static HookProc _hpk; // hookProcKeyboard

        public static void HookKeyboard()
        {
            if (_hhkk != IntPtr.Zero)
            {
                throw new InvalidOperationException();
            }

            _hpk = new HookProc(LowLevelKeyboardProc);
            _hhkk = SetWindowsHookEx(WH_KEYBOARD_LL, _hpk, IntPtr.Zero, 0);
        }

        private static IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (IsForeground && nCode == HC_ACTION)
            {
                var state = wParam.ToInt32();
                if (state == WM_KEYDOWN || state == WM_SYSKEYDOWN)
                {
                    //var keyData = (Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT)) as KBDLLHOOKSTRUCT)?.vkCode;
                    var e = new KeyEventArgs((Keys) Marshal.ReadInt32(lParam));
                    KeyDown?.Invoke(null, e);
                    if (e.SuppressKeyPress)
                    {
                        return (IntPtr) 1;
                    }
                }
            }
            return CallNextHookEx(_hhkk, nCode, wParam, lParam);
        }

        public static void UnhookKeyboard()
        {
            if (_hhkk != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hhkk);
                _hpk = null;
            }
        }

        #endregion

        #region Listen(Action<string[]> onSignal)

        public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private static IntPtr MessageServerHandle;

        public static void RunMessageServer()
        {
            // 根据 dll 的 fileVersion，按版本将其解压到不重叠的路径：
            // 旧版本的 dll 可能会以相同的名称保留在系统内核中
            IO.FileEx.Extract(Assembly.GetExecutingAssembly().GetManifestResourceStream("insoLyrics.Server.dll"), Constants._Server);
            var dest = Constants._Server + "." + FileVersionInfo.GetVersionInfo(Constants._Server).FileVersion;
            IO.FileEx.Move(Constants._Server, dest);

            MessageServerHandle = LoadLibrary(dest);
        }

        private static IntPtr LoadLibrary(string dllPath)
        {
            uint dwExitCode;

            var hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, Process.Id);

            var szFileName = Marshal.StringToHGlobalUni(dllPath);
            var nFileNameLength = GlobalSize(szFileName);
            var pParameter = VirtualAllocEx(hProcess, IntPtr.Zero, nFileNameLength, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
            WriteProcessMemory(hProcess, pParameter, szFileName, nFileNameLength, out _);
            Marshal.FreeHGlobal(szFileName);

            var pThreadProc = GetProcAddress(GetModuleHandle(ExternDll.Kernel32), "LoadLibraryW");

            var hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, pThreadProc, pParameter, 0, IntPtr.Zero);
            WaitForSingleObject(hThread, INFINITE);
            GetExitCodeThread(hThread, out dwExitCode);
            CloseHandle(hThread);

            VirtualFreeEx(hProcess, pParameter, 0, MEM_RELEASE);

            CloseHandle(hProcess);

            return new IntPtr(dwExitCode);
        }

        private static void ListenMessage()
        {
            // 在后台接收和转发来自服务器的数据
            using (var pipe = new NamedPipeClientStream(".", "osu!Lyrics", PipeDirection.In, PipeOptions.None))
            using (var sr = new StreamReader(pipe, Encoding.Unicode))
            {
                pipe.Connect();
                while (pipe.IsConnected && !sr.EndOfStream)
                {
                    var e = new MessageReceivedEventArgs(sr.ReadLine());
                    //MessageReceived?.BeginInvoke(null, e, null, null);
                    MessageReceived?.Invoke(null, e);
                }
            }
        }

        public static Task ListenMessageAsync() => Task.Run(() => ListenMessage());

        #endregion

        #region WindowInfo()

        public static Point ClientLocation
        {
            get
            {
                var location = Point.Empty;
                ClientToScreen(Process.MainWindowHandle, ref location);
                return location;
            }
        }

        public static Size ClientSize
        {
            get
            {
                GetClientRect(Process.MainWindowHandle, out Rectangle rect);
                return rect.Size;
            }
        }

        public static Rectangle ClientBounds => new Rectangle(ClientLocation, ClientSize);

        #endregion
    }
}
