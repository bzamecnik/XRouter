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
        public void SetRemoteProperty()
        {
            FullService service = new FullService() { Property = 1 };
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
            proxy.Property = 2;
            int value = proxy.Property;

            Assert.Equal(2, value);
        }

        [Fact]
        public void CallRemoteMethod()
        {
            FullService service = new FullService();
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
            string result = proxy.Method("foo");

            Assert.Equal("FOO", result);
        }

        // TODO:
        // - throws and exception when remotely calling Invoke() method
        //   - SampleEventHandler is in object[] and is treated as object
        //     (which is marked [Serializable]), but SampleEventHandler
        //     itself is not marked [Serializable]
        //   - should be resolved by fixing ticket #31
        [Fact]
        public void CallRemoteEvent()
        {
            FullService service = new FullService();
            RemoteObjectAddress address = ObjectServer.PublishObject(service);
            IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);
            string result = string.Empty;
            // add event handler remotely
            proxy.Event += (sender, args) => { result = args.Text; };
            // raise the event locally on the remote site
            // (it calls our local handler attached to the event
            // which sets the result to the 'result' variable)
            proxy.RaiseEvent("foo");

            Assert.Equal("foo", result);
        }

        // NOTE: It is not possible to remotely raise the event by direct
        // invocation of the event. The event in the remote proxy is
        // accessible only through an interface and it supports only
        // adding and removing handlers, not invocation.
        // So the following code doesn't work in C#:

        //[Fact]
        //public void RaiseRemoteEvent()
        //{
        //    FullService service = new FullService();
        //    string result = string.Empty;
        //    // add event handler locally
        //    service.Event += (sender, args) => { result = args.Text; };
        //    RemoteObjectAddress address = ObjectServer.PublishObject(service);
        //    IFullService proxy = RemoteObjectProxyProvider.GetProxy<IFullService>(address);

        //    // raise the event locally on the remote site
        //    if (proxy.Event != null)
        //    {
        //        proxy.Event(this, new SampleEventArgs("foo"));
        //    }
        //    proxy.Event("foo");

        //    Assert.Equal("foo", result);
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
            // sample property
            int Property { get; set; }
            // sample event
            event SampleEventHandler Event;
            // method from which the event is raised
            void RaiseEvent(string input);
            // sample method
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

            public event SampleEventHandler Event = delegate { };

            public void RaiseEvent(string text)
            {
                if (Event != null)
                {
                    Event(this, new SampleEventArgs(text));
                }
            }

            public string Method(string input)
            {
                return input.ToUpper();
            }
        }

        public delegate void SampleEventHandler(object sender, SampleEventArgs e);

        [Serializable]
        public class SampleEventArgs : EventArgs
        {
            public string Text { get; set; }
            public SampleEventArgs(string text)
            {
                Text = text;
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
