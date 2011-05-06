using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ObjectRemoter.Test
{
    class RemoteObjectProxyProviderTest
    {
        #region Happy-day test methods

        [Fact, Trait("Category", "Network")]
        public void GetProxy()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            IInvocable proxy = RemoteObjectProxyProvider.GetProxy<IInvocable>(
                address, typeof(IInvocable));
            Assert.NotNull(proxy);
        }

        // required interface type is descendant of type parameter
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithDescendantRequiredInterface()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            IRemotelyReferable proxy = RemoteObjectProxyProvider.GetProxy<IRemotelyReferable>(
                    address, typeof(IInvocable));
            Assert.NotNull(proxy);
        }

        [Fact, Trait("Category", "Network")]
        public void GetProxyWithNullInterfaceType()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            IInvocable proxy = RemoteObjectProxyProvider.GetProxy<IInvocable>(address, null);
            Assert.NotNull(proxy);
        }

        // TODO:
        // - try a class with custom interface
        // - local address

        #endregion

        #region Bad-day test methods

        [Fact, Trait("Category", "Network")]
        public void GetProxyWithNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RemoteObjectProxyProvider.GetProxy<IInvocable>(null, typeof(IInvocable))
            );
        }

        // requiredInterface is not an interface
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithNonInterfaceType()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            Assert.Throws<InvalidOperationException>(() =>
                RemoteObjectProxyProvider.GetProxy<IInvocable>(
                    address, typeof(SampleInvocable))
            );
        }

        // type parameter type is not an interface
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithNonInterfaceTypeParameter()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            Assert.Throws<InvalidOperationException>(() =>
                RemoteObjectProxyProvider.GetProxy<SampleInvocable>(
                    address, typeof(IInvocable))
            );
        }

        // required interface type is not assignable from proxy type
        // (is completely different)
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithIncompatibleInterface()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            // pass a different interface parameter then is the interface of the proxy
            // (eg. IRemotelyCloneable instead of IInvocable)
            Assert.Throws<ArgumentException>(() =>
                RemoteObjectProxyProvider.GetProxy<IInvocable>(
                    address, typeof(IRemotelyCloneable))
            );
        }

        // required interface type is not assignable from proxy type
        // (is antecendant of type parameter)
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithAntecendantRequiredInterface()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            Assert.Throws<ArgumentException>(() =>
                RemoteObjectProxyProvider.GetProxy<IInvocable>(
                address, typeof(IRemotelyReferable)));
        }

        // TODO:
        // - ProxyInterceptor - System.Net.Sockets.SocketException

        #endregion

        #region Helper classes and interfaces

        private class SampleInvocable : IInvocable
        {
            public object Invoke(object[] arguments)
            {
                return null;
            }
        }

        #endregion
    }
}
