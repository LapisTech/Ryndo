using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Ryndo
{
    class Win32API
    {
        // Load Win32 API
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr handle, int command);
        [DllImport("user32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, UInt32 hWndInsertAfter, int x, int y, int width, int height, int flags);
        [DllImport("user32")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        public static string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length <= 0) { return ""; }
            StringBuilder buffer = new StringBuilder(length + 1);
            GetWindowText(hWnd, buffer, buffer.Capacity);
            return buffer.ToString();
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // Set Win32 number.
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int SW_MAXIMIZE = 3;
        public const int WS_SYSMENU = 0x00080000;
        public const int WS_CAPTION = 0x00C00000;
        public const int WS_SIZEBOX = 0x00040000;
        public const int WS_DLGFRAME = 0x400000;
        public const int WS_CHILD = 0x40000000;
        public const int SWP_NOREPOSITION = 0x0200;
        public const int SWP_NOSENDCHANGING = 0x0400;
        public const int SWP_SHOWWINDOW = 0x0040;
        public const int SWP_NOACTIVATE = 0x0010;
    }
}
