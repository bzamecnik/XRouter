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
        public DateTime Date { set; get; }
       
        public int Table { set; get; }
        
        public List<Item> Items = new List<Item>();
    }   
}
