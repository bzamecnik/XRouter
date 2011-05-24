namespace ObjectRemoter.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class RemoteObjectProxyProviderTest
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
            InvokeSampleInvokable(proxy);
        }

        [Fact, Trait("Category", "Network")]
        public void GetProxyTwice()
        {
            SampleInvocable sampleInvocable = new SampleInvocable();
            RemoteObjectAddress address = ObjectServer.PublishObject(sampleInvocable);
            IInvocable proxy1 = RemoteObjectProxyProvider.GetProxy<IInvocable>(
                address, typeof(IInvocable));
            IInvocable proxy2 = RemoteObjectProxyProvider.GetProxy<IInvocable>(
                address, typeof(IInvocable));

            Assert.NotNull(proxy1);
            Assert.NotNull(proxy2);
            Assert.Equal(proxy1, proxy2);
            InvokeSampleInvokable(proxy1);
            InvokeSampleInvokable(proxy2);
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
            InvokeSampleInvokable(proxy);
        }

        // published object and proxy implement a custom interface
        [Fact, Trait("Category", "Network")]
        public void GetProxyWithCustomInterface()
        {
            ToUpperTextFilter obj = new ToUpperTextFilter();
            RemoteObjectAddress address = ObjectServer.PublishObject(obj);
            ITextFilter proxy = RemoteObjectProxyProvider.GetProxy<ITextFilter>(
                    address, typeof(ITextFilter));

            Assert.NotNull(proxy);
            Assert.Equal("FOO", proxy.Filter("Foo"));
        }

        // TODO:
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

        #region Using the remoted objects

        [Fact]
        public void GetProperty()
        {
            FullService service = new FullService() { Property = 42 };
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);

            Assert.Equal(42, proxy.Property);
        }

        [Fact]
        public void SetProperty()
        {
            FullService service = new FullService() { Property = 1 };
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
            proxy.Property = 2;
            int value = proxy.Property;

            Assert.Equal(2, value);
        }

        [Fact]
        public void CallMethod()
        {
            FullService service = new FullService();
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
            string result = proxy.Method("foo");

            Assert.Equal("FOO", result);
        }

        //[Fact]
        //public void CallEvent()
        //{
        //    FullService service = new FullService();
        //    service.Event += (value) => { Property = value; };
        //    RemoteObjectAddress address = ObjectServer.PublishObject(service);
        //    IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
        //    string result = proxy.Method("foo");

        //    Assert.Equal("FOO", result);
        //}

        #endregion

        #endregion

        #region Helper classes and interfaces

        private class SampleInvocable : IInvocable
        {
            public object Invoke(object[] arguments)
            {
                if ((arguments.Length > 0) && (arguments[0] is string))
                {
                    return ((string)arguments[0]).ToUpper();
                }
                return null;
            }
        }

        public interface ITextFilter : IRemotelyReferable
        {
            string Filter(string text);
        }

        // a class with custom interface derived from IRemotelyReferable
        private class ToUpperTextFilter : ITextFilter
        {
            public string Filter(string text)
            {
                return text.ToUpper();
            }
        }

        // service with all supported member categories
        // (methods, properties, events)
        public interface IFullService : IRemotelyReferable
        {
            int Property { get; set; }
            event EventHandler Event;
            string Method(string input);
        }

        public class FullService : IFullService
        {
            private int property;
            public int Property
            {
                get { return property; }
                set { property = value; }
            }

            public event EventHandler Event = delegate { };

            public string Method(string input)
            {
                return input.ToUpper();
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Test if invoking the SampleInvocable instance works.
        /// </summary>
        /// <param name="sampleInvocable">Can be real local instance or remote
        /// proxy.</param>
        private static void InvokeSampleInvokable(IInvocable sampleInvocable)
        {
            Assert.Equal("FOO", sampleInvocable.Invoke(new[] { "Foo" }));
        }

        #endregion
    }
}
