using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XRouter.Common.Test
{
    class TraceLogReaderTest
    {
        public void ReadLogs()
        {
            TraceLogReader reader = new TraceLogReader(
                "TestLogs", "xroutermanager");
            var entries = reader.GetEntries(
                new DateTime(2011, 1, 1),
                new DateTime(2012, 1, 1),
                LogLevelFilters.Error | LogLevelFilters.Warning | LogLevelFilters.Info,
                int.MaxValue, 1);
            int total = entries.Count();
            Console.WriteLine("Total: " + total);
        }
    }
}
