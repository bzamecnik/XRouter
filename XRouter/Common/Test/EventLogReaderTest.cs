using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XRouter.Common.Test
{
    class EventLogReaderTest
    {
        public void ReadLogs()
        {
            EventLogReader reader = new EventLogReader(
                "TestLogs", "xrouter");
            var entries = reader.GetEntries(
                new DateTime(2011, 1, 1),
                new DateTime(2012, 1, 1),
                LogLevelFilters.Error | LogLevelFilters.Warning | LogLevelFilters.Info,
                int.MaxValue, 1);
            int total = entries.Count();
            Console.WriteLine("Total: " + total);

            entries = reader.GetEntries(
                new DateTime(2011, 1, 1),
                new DateTime(2012, 1, 1),
                LogLevelFilters.Error,
                int.MaxValue, 1);
            int error = entries.Count();
            Console.WriteLine("Error: " + total);
            entries = reader.GetEntries(
                new DateTime(2011, 1, 1),
                new DateTime(2012, 1, 1),
                LogLevelFilters.Warning,
                int.MaxValue, 1);
            int warning = entries.Count();
            Console.WriteLine("Warning: " + total);
            entries = reader.GetEntries(
                new DateTime(2011, 1, 1),
                new DateTime(2012, 1, 1),
                LogLevelFilters.Info,
                int.MaxValue, 1);
            int info = entries.Count();
            Console.WriteLine("Info: " + total);
            Assert.Equal(total, error + warning + info);
        }
    }
}
