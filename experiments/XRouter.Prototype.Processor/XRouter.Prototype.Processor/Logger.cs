using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XRouter.Prototype.Processor
{
    static class Logger
    {
        public static void LogInfo(String message)
        {
            Console.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "] " + DateTime.Now + " I " + message);
        }

        public static void LogError(String error)
        {
            Console.WriteLine("[" + Thread.CurrentThread.ManagedThreadId + "] " + DateTime.Now + " E " + error);
        }
    }
}
