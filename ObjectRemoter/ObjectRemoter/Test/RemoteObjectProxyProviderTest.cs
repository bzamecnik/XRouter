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
        #endregion

        #region Bad-day test methods

        [Fact]
        public void GetProxyWithNullAddress()
        {
            Assert.Throws<ArgumentNullException>(() =>
                RemoteObjectProxyProvider.GetProxy<IInvocable>(null, typeof(IInvocable))
            );
        }

        #endregion
    }
}
