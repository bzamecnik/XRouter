using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Threading;

namespace XRouter.Gui.Utils
{
    class ThreadUtils
    {
        public static void InvokeLater(TimeSpan delay, Action action)
        {
            Task.Factory.StartNew(delegate {
                Thread.Sleep(delay);
                App.Current.Dispatcher.BeginInvoke(action);
            });
        }
    }
}
