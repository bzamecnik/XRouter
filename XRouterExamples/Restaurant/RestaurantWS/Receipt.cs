using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace XRouterWS
{   
    public class Receipt
    {      
        public DateTime date { set; get; }
       
        public int table { set; get; }
        
        public List<Item> items = new List<Item>();
    }   
}
