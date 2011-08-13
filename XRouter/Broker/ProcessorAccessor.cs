using System;
using XRouter.Common;
using XRouter.Common.ComponentInterfaces;

namespace XRouter.Broker
{
    /// <summary>
    /// A proxy for accessing a processor. It is able to cache some frequently
    /// queried values such as its utilization.
    /// </summary>
    /// <seealso cref="XRouter.Common.ComponentInterfaces.IProcessorService"/>
    class ProcessorAccessor : ComponentAccessor
    {
        private double? utilizationCache;
        private DateTime lastUtilizationCacheUpdate;

        private IProcessorService processor;

        private TimeSpan UtilizationCacheTimeout {
            get {
                // TODO: consider using configuration
                return TimeSpan.FromSeconds(10);
            }
        }

        public ProcessorAccessor(string componentName, IProcessorService processor, ApplicationConfiguration configuration)
            : base(componentName, processor, configuration)
        {
            this.processor = processor;
        }

        public double GetUtilization()
        {
            if (utilizationCache.HasValue) {
                TimeSpan elapsedSinceLastCacheUpdate = lastUtilizationCacheUpdate - DateTime.Now;
                if (elapsedSinceLastCacheUpdate <= UtilizationCacheTimeout) {
                    return utilizationCache.Value;
                }
            }

            double utilization = processor.GetUtilization();
            utilizationCache = utilization;
            lastUtilizationCacheUpdate = DateTime.Now;
            return utilization;
        }

        public void AddWork(Token token)
        {
            processor.AddWork(token);
        }
    }
}
