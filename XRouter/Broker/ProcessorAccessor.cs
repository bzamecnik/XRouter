using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common;

namespace XRouter.Broker
{
    class ProcessorAccessor : ComponentAccessor
    {
        private double? utilizationCache;
        private DateTime lastUtilizationCacheUpdate;

        private TimeSpan UtilizationCacheTimeout {
            get {
                // TODO: consider using configuration
                return TimeSpan.FromSeconds(10);
            }
        }

        public ProcessorAccessor(string componentName, ApplicationConfiguration configuration)
            : base(componentName, configuration)
        {
        }

        public double GetUtilization()
        {
            if (utilizationCache.HasValue) {
                TimeSpan elapsedSinceLastCacheUpdate = lastUtilizationCacheUpdate - DateTime.Now;
                if (elapsedSinceLastCacheUpdate <= UtilizationCacheTimeout) {
                    return utilizationCache.Value;
                }
            }

            IProcessorService processor = GetComponent<IProcessorService>();
            double utilization = processor.GetUtilization();
            utilizationCache = utilization;
            lastUtilizationCacheUpdate = DateTime.Now;
            return utilization;
        }

        public void AddWork(Token token)
        {
            IProcessorService processor = GetComponent<IProcessorService>();
            processor.AddWork(token);
        }
    }
}
