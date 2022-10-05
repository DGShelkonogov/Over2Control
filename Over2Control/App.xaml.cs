using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Over2Control
{
    public partial class App : Application
    {
        protected Mutex Mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            Mutex = new Mutex(true, ResourceAssembly.GetName().Name);
            if (!Mutex.WaitOne())
            {
                Current.Shutdown();
                return;
            }
            else
            {
                new MainWindow().Show();
                ShutdownMode = ShutdownMode.OnLastWindowClose;
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Mutex != null)
            {
                Mutex.ReleaseMutex();
            }

            base.OnExit(e);
        }
    }

}
