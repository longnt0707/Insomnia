using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Insomnia
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InsomniaAppContext());
        }
    }

    public class InsomniaAppContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        //@d https://stackoverflow.com/questions/4722198/checking-if-my-windows-application-is-running
        private const string AppId = "98-5F-D3-3B-AA-A6_ApoChan_AnhBijin_longnt0707_Insomnia_Mdj1V6bumTzBgIuDc6I";
        private readonly Semaphore instancesAllowed = new Semaphore(1, 1, AppId);
        private bool WasRunning { set; get; }

        public InsomniaAppContext()
        {
            //See if application is already running on the system
            if (this.instancesAllowed.WaitOne(1000))
            {
                // Initialize Tray Icon
                trayIcon = new NotifyIcon()
                {
                    Icon = Properties.Resources.Insomnia,
                    ContextMenu = new ContextMenu(new MenuItem[] {
                    new MenuItem("Exit", Exit)
                }),
                    Visible = true
                };

                PowerHelper.ForceSystemAwake();
                this.WasRunning = true;
                return;
            }
            else
            {
                //Display
                MessageBox.Show("An instance of Insomnia is already running");

                //Exit out otherwise
                this.Exit(null, null);
            }
        }

        void Exit(object sender, EventArgs e)
        {
            //Decrement the count if app was running
            if (this.WasRunning)
            {
                this.instancesAllowed.Release();
            }

            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            PowerHelper.ResetSystemDefault();

            Application.Exit();
        }
    }

    //@d https://stackoverflow.com/questions/24869508/moving-mouse-to-stop-monitor-from-sleeping
    //@d https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate
    public class PowerHelper
    {
        public static void ForceSystemAwake()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS |
                                                NativeMethods.EXECUTION_STATE.ES_AWAYMODE_REQUIRED |
                                                NativeMethods.EXECUTION_STATE.ES_DISPLAY_REQUIRED |
                                                NativeMethods.EXECUTION_STATE.ES_SYSTEM_REQUIRED);
        }

        public static void ResetSystemDefault()
        {
            NativeMethods.SetThreadExecutionState(NativeMethods.EXECUTION_STATE.ES_CONTINUOUS);
        }
    }

    internal static partial class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001

            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }
    }
}
