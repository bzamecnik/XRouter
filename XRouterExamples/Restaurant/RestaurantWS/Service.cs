using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouterWS
{
    [ServiceBehavior(InstanceContextMode = System.ServiceModel.InstanceContextMode.Single)]
    class Service : IService
    {
        private List<Receipt> receipts = new List<Receipt>();
        private readonly XDocument menu;

        private class MenuItem
        {
            public string Name;
            public int Price;
        }

        private MenuItem GetReceiptItem(string itemID)
        {
            MenuItem menuItem = new MenuItem();

            XElement result = menu.XPathSelectElements("//*[@id=\"" + itemID + "\"]").FirstOrDefault();
            if (result == null)
            {
                return null;
            }

            menuItem.Name = result.Element(XName.Get("name")).Value;
            menuItem.Price = int.Parse(result.Element(XName.Get("price")).Value);

            return menuItem;
        }

        public Service()
        {
            menu = XDocument.Load(@"..\..\..\Data\RestaurantMenu.xml");
        }
      
        public void SaveOrder(Order order)
        {
            Console.WriteLine("save order");
            Console.WriteLine(order.item);
            Console.WriteLine(order.table);

            MenuItem menuItem = GetReceiptItem(order.item);
            if (menuItem == null) //unknown food or drink
            {
                return;
            }

            Receipt receipt = null;
            foreach (Receipt rcpt in receipts)
            {
                if (rcpt.table == order.table)
                {
                    receipt = rcpt;
                    break;
                }
            }
            if (receipt == null)
            {
                receipt = new Receipt();
                receipt.table = order.table;
                receipts.Add(receipt);
            }

            Item item = null;
            foreach (Item itm in receipt.items)
            {
                if (itm.Name == menuItem.Name)
                {
                    item = itm;
                    break;
                }
            }
            if (item == null)
            {
                item = new Item();
                item.Name = menuItem.Name;
                item.Quantity = 0;
                item.TotalPrice = 0;
                receipt.items.Add(item);
            }

            item.Quantity += 1;
            item.TotalPrice = item.Quantity * menuItem.Price;
        }

        public Receipt GetReceipt(Payment payment)
        {
            Console.WriteLine("get receipt");
            Console.WriteLine(payment.table);

            //find, delete and send
            Receipt receipt = null;
            foreach (Receipt rcpt in receipts)
            {
                if (rcpt.table == payment.table)
                {
                    receipt = rcpt;
                    receipts.Remove(rcpt);
                    break;
                }
            }
            if (receipt == null)
            {
                receipt = new Receipt();
                receipt.table = payment.table;
            }
            receipt.date = DateTime.Now;

            return receipt;
        }
    }
}
