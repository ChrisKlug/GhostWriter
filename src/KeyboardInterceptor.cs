using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace GhostWriter
{
    class KeyboardInterceptor
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYUP = 0x0101;
        private static IntPtr hookID = IntPtr.Zero;
        private HookHandlerDelegate callbackDelegate;

        public KeyboardInterceptor()
        {

            Process curProcess = Process.GetCurrentProcess();
            ProcessModule curModule = curProcess.MainModule;

            callbackDelegate = new HookHandlerDelegate(HookCallback);

            hookID = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, callbackDelegate, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (lParam.vkCode == 44)
            {
                if (wParam == (IntPtr)WM_KEYUP)
                {
                    // Print Screen key released, write text
                    Timer timer = null;
                    timer = new Timer(x => {
                        OnPrintScreenKeyPressed();
                        timer.Dispose();
                    }, null, 100, Timeout.Infinite);
                }

                // Ignore Print Key
                return (IntPtr)1;
            }

            // Pass key to next application
            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, ref lParam);
        }

        protected virtual void OnPrintScreenKeyPressed()
        {
            PrintScreenKeyPressed?.Invoke(this, new EventArgs());
        }

        public event EventHandler PrintScreenKeyPressed;

        internal delegate IntPtr HookHandlerDelegate(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        // Structure returned by the hook whenever a key is pressed
        internal struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        [ComVisible(false), System.Security.SuppressUnmanagedCodeSecurity()]
        internal class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

            //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            //[return: MarshalAs(UnmanagedType.Bool)]
            //public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

            //[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
            //public static extern short GetKeyState(int keyCode);

        }
    }
}
