namespace ObjectRemoter.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class ServiceRemoterTest
    {
        #region Happy-day test methods

        [Fact]
        public void PublishService()
        {
            Uri serviceUri = ServiceRemoter.PublishService<ITextFilterService>(new ToUpperTextFilterService());

            Assert.NotNull(serviceUri);
        }

        [Fact]
        public void GetAndUseServiceProxy()
        {
            Uri serviceUri = ServiceRemoter.PublishService<ITextFilterService>(new ToUpperTextFilterService());
            ITextFilterService serviceProxy = ServiceRemoter.GetServiceProxy<ITextFilterService>(serviceUri);

            Assert.NotNull(serviceProxy);
            Assert.Equal("FOO", serviceProxy.Filter("Foo"));
        }

        // if the ServiceRemoter takes proxy for any instance of the published
        // object type instead of a particular one this test fails
        [Fact]
        public void PublishAndUseTwoServiceInstances()
        {
            var service1 = new ConstantTextFilterService("foo");
            var service2 = new ConstantTextFilterService("bar");
            Uri serviceUri1 = ServiceRemoter.PublishService<ITextFilterService>(service1);
            Uri serviceUri2 = ServiceRemoter.PublishService<ITextFilterService>(service2);
            ITextFilterService serviceProxy1 = ServiceRemoter.GetServiceProxy<ITextFilterService>(serviceUri1);
            ITextFilterService serviceProxy2 = ServiceRemoter.GetServiceProxy<ITextFilterService>(serviceUri2);

            Assert.NotNull(serviceProxy1);
            Assert.NotNull(serviceProxy2);
            Assert.Equal("foo", serviceProxy1.Filter("..."));
            Assert.Equal("bar", serviceProxy1.Filter("..."));
        }

        #endregion

        #region Bad-day test methods

        [Fact]
        public void PublishNullObject()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ServiceRemoter.PublishService<ITextFilterService>(null));
        }

        #endregion

        #region Helper methods

        #endregion

        #region Helper classes

        public interface ITextFilterService : IRemotelyReferable
        {
            string Filter(string text);
        }

        // a class with custom interface derived from IRemotelyReferable
        private class ToUpperTextFilterService : ITextFilterService
        {
            public string Filter(string text)
            {
                return text.ToUpper();
            }
        }

        private class ConstantTextFilterService : ITextFilterService
        {
            private string constantTest;
            public ConstantTextFilterService(string text)
            {
                constantTest = text;
            }

            public string Filter(string text)
            {
                return constantTest;
            }
        }

        #endregion
    }
}
