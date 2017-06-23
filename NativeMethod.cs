using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AccelerationSensorViewer
{
    public static class NativeMethod
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String sClassName, String sWindowText);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool PostMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);


    }
}
