namespace ObjectRemoter.Test.RemoteCommunication
{
    using System;
    using ObjectRemoter.RemoteCommunication;
    using Xunit;
    using Xunit.Extensions;

    public class ServerAddressTest
    {
        #region Happy-day tests

        [Fact]
        public void CreateServerAddressWithIpAdress()
        {
            Uri uri = new Uri("tcp://10.20.30.40:1024/");
            ServerAddress address = new ServerAddress(uri);

            Assert.Equal(uri, address.Url);
            Assert.Equal(1024, address.Port);
            Assert.Equal("10.20.30.40", address.IPAddress.ToString());
        }

        [Fact]
        public void CreateServerAddressWithLocalhostHostname()
        {
            Uri uri = new Uri("tcp://localhost:1024/");
            ServerAddress address = new ServerAddress(uri);

            Assert.Equal(uri, address.Url);
            Assert.Equal(1024, address.Port);
            Assert.Equal("127.0.0.1", address.IPAddress.ToString());
        }

        [Fact]
        public void CreateServerAddressWithoutPort()
        {
            Uri uri = new Uri("tcp://localhost/");
            ServerAddress address = new ServerAddress(uri);

            Assert.Equal(uri, address.Url);
            Assert.Equal(uri.Port, address.Port);
            Assert.Equal("127.0.0.1", address.IPAddress.ToString());
        }

        [Fact]
        public void CreateLocalServerAddress()
        {
            ServerAddress address = ServerAddress.GetLocalServerAddress();

            //Console.WriteLine("{0}, {1}, {2}", address.Url, address.IPAddress, address.Port);
            Assert.NotNull(address);
            Assert.NotNull(address.Url);
            Assert.NotNull(address.IPAddress);
            Assert.NotNull(address.Url);
            Assert.Equal(address.Url.Port, address.Port);
        }

        [Theory]
        [InlineData("tcp://10.20.30.40:1024/")]
        [InlineData("tcp://127.0.0.1/")]
        [InlineData("tcp://localhost/")]
        [InlineData("tcp://localhost:1234/")]
        public void SerializeAndDeserializeAddress(string uri)
        { 
            ServerAddress address = new ServerAddress(new Uri(uri));
            string serializedAddress = address.Serialize();
            ServerAddress deserializedAddress = ServerAddress.Deserialize(serializedAddress);

            Assert.Equal(address, deserializedAddress);
        }

        // TODO:
        // - there is a problem that we can't start ObjectServer with arbitrary
        //   address/port and we cannot 
        [Theory]
        [InlineData("tcp://10.20.30.40:1024/", false)]
        [InlineData("tcp://127.0.0.1/", false)]
        public void CheckIsLocalProperty(string uri, bool expectedIsLocal)
        {
            ServerAddress address = new ServerAddress(new Uri(uri));
            Assert.Equal(expectedIsLocal, address.IsLocal);
        }

        [Fact]
        [Trait("Category", "Network")]
        [Trait("Category", "NeedsLocalHost")]
        [Trait("Category", "WhiteBox")]
        public void CheckIsLocalPropertyForObjectServerAdress()
        {
            RemoteObjectAddress address = ObjectServer.PublishObject(new SampleRemotelyReferable());
            Assert.Equal(true, address.ServerAddress.IsLocal);
        }

        #endregion

        #region Bad-day tests

        [Fact]
        public void DeserializeNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() => ServerAddress.Deserialize(null));
        }

        [Fact]
        public void CreateNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() => new ServerAddress(null));
        }

        #endregion

        #region Helper classes

        class SampleRemotelyReferable : IRemotelyReferable
        {
        }

        #endregion
    }
}
