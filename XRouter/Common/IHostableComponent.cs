using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Common
{
    public interface IHostableComponent
    {
        event Action<string> LogEventInfo;
        event Action<string> LogEventWarning;
        event Action<string> LogEventError;

        event Action<string> LogTraceInfo;
        event Action<string> LogTraceWarning;
        event Action<string> LogTraceError;
        event Action<Exception> LogTraceException;

        void Start(string componentName, IDictionary<string, string> settings);
        void Stop();
    }
}
