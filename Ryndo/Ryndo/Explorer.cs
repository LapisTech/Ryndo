using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Ryndo
{
    class Explorer
    {
        // Load Win32 API
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr handle, int command);
        [DllImport("user32.dll")]
        private static extern UInt32 SetWindowPos(IntPtr hWnd, UInt32 hWndInsertAfter, int x, int y, int width, int height, int flags);
        [DllImport("user32")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwLong);

        // Set Win32 number.
        private int GWL_STYLE = -16;
        private int SW_MAXIMIZE = 3;
        private int WS_SYSMENU = 0x00080000;
        private int WS_CAPTION = 0x00C00000;
        private int WS_SIZEBOX = 0x00040000;
        private int SWP_NOREPOSITION = 0x0200;
        private int SWP_NOSENDCHANGING = 0x0400;
        private int SWP_SHOWWINDOW = 0x0040;
        private int SWP_NOACTIVATE = 0x0010;

        // Data.
        private bool initFlag = false;
        private string path = "";
        System.Windows.Forms.Panel panel = null;
        Process process = null;

        public Explorer()
        {
        }

        public string GetDirName()
        {
            string[] dirs = this.path.Split('\\');
            return dirs[dirs.Length - 1];
        }

        public void OpenDir(string path)
        {
            this.path = path;

            ProcessStartInfo psi = new ProcessStartInfo(@"notepad");
            //ProcessStartInfo psi = new ProcessStartInfo(@"explorer");
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;

            process = Process.Start(psi);
            process.WaitForInputIdle();

            // Wait create window.
            while (process.MainWindowHandle == IntPtr.Zero && process.HasExited == false)
            {
                System.Threading.Thread.Sleep(1);
                process.Refresh();
            }

            this.initFlag = true;

            // Set window in panel.
            this.ChangeParent(this.panel, true);

            // Delete window style.
            var windowStyle = GetWindowLong(process.MainWindowHandle, GWL_STYLE);
            windowStyle &= ~(WS_SYSMENU | WS_CAPTION | WS_SIZEBOX);
            SetWindowLong(process.MainWindowHandle, GWL_STYLE, windowStyle);

            // Set window size;
            this.Maximize();

            // Window maximize.
            ShowWindow(process.MainWindowHandle, SW_MAXIMIZE);
        }

        public void ChangeParent(System.Windows.Forms.Panel panel, bool update = false)
        {
            this.panel = panel;
            if (!update) { return; }
            SetParent(process.MainWindowHandle, panel.Handle);
        }

        public void Maximize()
        {
            if (!this.initFlag) { return; }
            SetWindowPos(process.MainWindowHandle, 0, 0, 0, (int)this.panel.Width, (int)this.panel.Height, SWP_NOREPOSITION | SWP_NOSENDCHANGING | SWP_SHOWWINDOW | SWP_NOACTIVATE);
        }

        public void OnResize()
        {
            this.Maximize();
        }
    }
}

