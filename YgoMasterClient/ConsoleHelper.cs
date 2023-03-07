using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace YgoMasterClient
{
    class ConsoleHelper
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetConsoleOutputCP();

        private const UInt32 StdOutputHandle = 0xFFFFFFF5;

        private static IntPtr consoleHandle;
        private static StreamWriter output;

        private const int SW_SHOW = 5;
        private const int SW_HIDE = 0;

        private static string title;
        public static string Title
        {
            get
            {
                return consoleHandle == IntPtr.Zero ? title : title = Console.Title;
            }
            set
            {
                title = value;
                if (consoleHandle != IntPtr.Zero)
                {
                    Console.Title = value;
                }
            }
        }

        static ConsoleHelper()
        {
            consoleHandle = GetConsoleWindow();
            if (consoleHandle != IntPtr.Zero)
            {
                Console.Title = Title;

                Stream consoleOutputStream = Console.OpenStandardOutput();
                Encoding consoleEncoding = Encoding.GetEncoding(GetConsoleOutputCP());
                output = new StreamWriter(consoleOutputStream, consoleEncoding, 0x100);
                output.AutoFlush = true;

                // SuppressFinalize on m_consoleOutputWriter because all it will do is flush
                // and close the file handle. Because we have set AutoFlush the additional flush
                // is not required. The console file handle should not be closed, so we don't call
                // Dispose, Close or the finalizer.
                GC.SuppressFinalize(output);
            }
        }

        public static bool IsConsoleVisible
        {
            get { return (consoleHandle = GetConsoleWindow()) != IntPtr.Zero && IsWindowVisible(consoleHandle); }
        }

        public static void ToggleConsole()
        {
            consoleHandle = GetConsoleWindow();
            if (consoleHandle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                FreeConsole();
            }
        }

        public static void ShowConsole()
        {
            consoleHandle = GetConsoleWindow();
            if (consoleHandle == IntPtr.Zero)
            {
                AllocConsole();
                consoleHandle = GetConsoleWindow();
            }
            else
            {
                ShowWindow(consoleHandle, SW_SHOW);
            }

            if (consoleHandle != IntPtr.Zero)
            {
                Console.Title = title != null ? title : string.Empty;
            }
        }

        public static void HideConsole()
        {
            consoleHandle = GetConsoleWindow();
            if (consoleHandle != IntPtr.Zero)
            {
                ShowWindow(consoleHandle, SW_HIDE);
            }
        }

        public static void CloseConsole()
        {
            consoleHandle = GetConsoleWindow();
            if (consoleHandle != IntPtr.Zero)
            {
                FreeConsole();
            }
        }
    }
}
