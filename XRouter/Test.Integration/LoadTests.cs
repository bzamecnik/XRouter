using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace XRouter.Test.Integration
{
    public class LoadTests
    {
        [Fact]
        public void FloodWebService()
        {
            FloodWebClient floodClient = new FloodWebClient()
            {
                ServiceUri = "http://localhost:8123/FloodConsumerWebService/"
            };
            floodClient.SendFixedCountFlood(10000, 1000);
        }
    }
}
