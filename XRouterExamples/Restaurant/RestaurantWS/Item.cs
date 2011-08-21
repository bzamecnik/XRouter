using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace XRouterWS
{
    public class item
    {   
        public string name { set; get; }

        public int quantity { set; get; }
      
        public int totalPrice { set; get; }
    }
}
