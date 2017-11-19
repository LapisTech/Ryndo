using SHDocVw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryndo
{
    class Explorer
    {

        // Data.
        System.Windows.Forms.Panel panel = null;
        IntPtr handle = IntPtr.Zero;

        public Explorer()
        {
        }

        public string GetDirName()
        {
            string[] dirs = Win32API.GetWindowTitle(this.handle).Split('\\');
            return dirs[dirs.Length - 1];
        }

        public void OpenDir(string path)
        {
            IntPtr handle = this.startProcess();

            // Failure get Explorer window.
            if (handle == IntPtr.Zero) { return; }
            this.handle = handle;

            //Win32API.PostMessage(handle, 0x0100, 0x7a, 0);

            // Set window in panel.
            this.ChangeParent(this.panel, true);

            // Delete window style.
            int windowStyle = Win32API.GetWindowLong(this.handle, Win32API.GWL_STYLE);
            //Console.WriteLine("style:" + windowStyle);
            //Console.WriteLine("exstyle:"+ Win32API.GetWindowLong(this.handle, Win32API.GWL_EXSTYLE));
            //windowStyle &= ~(Win32API.WS_SYSMENU | Win32API.WS_CAPTION | Win32API.WS_SIZEBOX);
            windowStyle =Win32API.WS_DLGFRAME|Win32API.WS_CHILD;
            Win32API.SetWindowLong(this.handle, Win32API.GWL_STYLE, windowStyle);

            // Set window size;
            //this.Maximize();

            // Window maximize.
            //Win32API.ShowWindow(this.handle, Win32API.SW_MAXIMIZE);
        }
        
        private IntPtr startProcess()
        {
            //ProcessStartInfo psi = new ProcessStartInfo(@"notepad");
            ProcessStartInfo psi = new ProcessStartInfo(@"explorer");
            psi.WindowStyle = ProcessWindowStyle.Minimized;

            // Get explorer list.
            InternetExplorer[] explorers = this.getExplorerList();

            // Start explorer.
            Process process = Process.Start(psi);
            process.WaitForInputIdle();

            // Wait create window.
            /*while (process.MainWindowHandle == IntPtr.Zero && process.HasExited == false)
            {
                System.Threading.Thread.Sleep(1);
                process.Refresh();
            }*/
            /*while (process.HasExited == false)
            {
                System.Threading.Thread.Sleep(1);
                process.Refresh();
            }*/

            // Get create Explorer window.
            IntPtr handle = IntPtr.Zero;
            int trynum = 100;
            do
            {
                System.Threading.Thread.Sleep(1);
                handle = this.getExplorerHandle(explorers);
            } while (handle == IntPtr.Zero && 0 < trynum--);
            return handle;
        }

        private bool isExplorer(InternetExplorer ie)
        {
            return "explorer".Equals(Path.GetFileNameWithoutExtension(ie.FullName), StringComparison.OrdinalIgnoreCase);
        }

        private InternetExplorer[] getExplorerList()
        {
            ShellWindows shellWindows = new ShellWindows();
            InternetExplorer[] explorers = new InternetExplorer[shellWindows.Count];
            int count = 0;

            foreach (InternetExplorer ie in shellWindows)
            {
                if (this.isExplorer(ie))
                {
                    Console.WriteLine(Path.GetFileNameWithoutExtension(ie.FullName));
                    Console.WriteLine(ie.ToString());
                    explorers[count++] = ie;
                }
            }
            return explorers;
        }

        private IntPtr getExplorerHandle(InternetExplorer[] explorers)
        {
            ShellWindows shellWindows = new ShellWindows();

            foreach (InternetExplorer ie in shellWindows)
            {
                if (!this.isExplorer(ie) || 0 <= Array.IndexOf(explorers, ie)) { continue; }
                return (IntPtr)ie.HWND;
            }

            return IntPtr.Zero;
        }

        public void ChangeParent(System.Windows.Forms.Panel panel, bool update = false)
        {
            this.panel = panel;
            if (!update || this.handle == IntPtr.Zero) { return; }
            Win32API.SetParent(this.handle, panel.Handle);
        }

        public void Maximize()
        {
            if (this.handle == IntPtr.Zero) { return; }
            Win32API.SetWindowPos(this.handle, 0, 0, 0, (int)this.panel.Width, (int)this.panel.Height, Win32API.SWP_NOREPOSITION | Win32API.SWP_NOSENDCHANGING | Win32API.SWP_SHOWWINDOW | Win32API.SWP_NOACTIVATE);
        }

        public void OnResize()
        {
            this.Maximize();
        }
    }
}

