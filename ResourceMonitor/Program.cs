using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;
using PavoStudio.ExApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ResourceMonitor
{
    static class Program
    {
        public const int AppClose = 1992;
        public const int ChangeSettingData = 2000;

        private static ResMonitor monitor;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if !DEBUG
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
#endif
            monitor = new ResMonitor();
            Messenger.AddListener(OnMessage, LocalMsg.OnOpen, LocalMsg.OnClose, LocalMsg.OnError, AppClose, ChangeSettingData);
            ExApiClient.Start(args);
            Application.Run();
        }

        static void OnMessage(BaseMessage bm)
        {
            //Console.WriteLine(bm.msg);
            switch (bm.msg)
            {
                case LocalMsg.OnOpen:
                    monitor.Start();
                    break;
                case LocalMsg.OnError:
                case LocalMsg.OnClose:
                case AppClose:
                    Messenger.RemoveListener(OnMessage, LocalMsg.OnOpen, LocalMsg.OnClose, LocalMsg.OnError, AppClose, ChangeSettingData);
                    monitor.Stop();
                    ExApiClient.Stop();
                    Application.Exit();
                    break;
                case ChangeSettingData:
                    UnityMessage um = bm.GetData<UnityMessage>();
                    monitor.SetInterval(um.i2);
                    break;
            }
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Application.Exit();
        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Environment.Exit(0);
        }
    }
}
