using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace XRouterWS
{
    public class Item
    {   
        public string Name { set; get; }

        public int Quantity { set; get; }
      
        public int TotalPrice { set; get; }
    }
}
