using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace XRouterWS
{    
    public class Order
    {          
        public int table { set; get; }
               
        public string item { set; get; }
    }
}
