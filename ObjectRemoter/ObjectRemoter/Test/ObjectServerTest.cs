namespace ObjectRemoter.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class ObjectServerTest
    {
        #region Happy-day test methods

        [Fact, Trait("Category", "Network")]
        public void PublishSingleObjectOnce()
        {
            var obj = new SampleRemotelyReferable();
            RemoteObjectAddress address = ObjectServer.PublishObject(obj);

            Assert.NotNull(address);
        }

        [Fact, Trait("Category", "Network")]
        public void PublishSingleObjectTwice()
        {
            var obj = new SampleRemotelyReferable();
            RemoteObjectAddress address1 = ObjectServer.PublishObject(obj);
            // if the object is published again it should get the same address
            // instead of creating a new one
            RemoteObjectAddress address2 = ObjectServer.PublishObject(obj);

            Assert.Equal(address1, address2);
        }

        [Fact, Trait("Category", "Network")]
        public void PublishSampleRemoteProxy()
        {
            var obj = new SampleRemotelyReferable();
            RemoteObjectAddress objAddress = ObjectServer.PublishObject(obj);
            var proxy = new SampleRemoteProxy(objAddress);
            RemoteObjectAddress proxyAddress = ObjectServer.PublishObject(proxy);

            Assert.Equal(objAddress, proxyAddress);
        }

        #endregion

        #region Bad-day test methods

        [Fact]
        public void PublishNullObject()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ObjectServer.PublishObject(null));
        }

        #endregion

        #region Helper methods

        #endregion

        #region Helper classes

        class SampleRemotelyReferable : IRemotelyReferable
        {
        }

        class SampleRemoteProxy : IRemoteObjectProxy, IRemotelyReferable
        {
            private RemoteObjectAddress remoteObjectAddress;

            public RemoteObjectAddress RemoteObjectAddress
            {
                get { return remoteObjectAddress; }
                private set { remoteObjectAddress = value; }
            }

            public SampleRemoteProxy(RemoteObjectAddress address)
            {
                RemoteObjectAddress = address;
            }
        }

        #endregion
    }
}
