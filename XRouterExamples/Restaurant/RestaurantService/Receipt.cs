using System.Collections.Generic;

namespace XRouter.Examples.Restaurant.RestaurantService
{   
    public class Receipt
    {      
        public string Date { set; get; }
       
        public int Table { set; get; }
        
        public List<Item> Items = new List<Item>();
    }   
}
