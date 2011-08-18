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
        private List<Receipt> receipts;
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

            menuItem.Name = result.Attribute("Name").Value;
            menuItem.Price = int.Parse(result.Attribute("Price").Value);

            return menuItem;
        }

        public Service()
        {
            menu = XDocument.Load(@"..\..\..\Data\RestaurantMenu.xml");
        }
      
        public void SaveOrder(Order order)
        {
            Console.WriteLine("save order");
            Console.WriteLine(order.Item);
            Console.WriteLine(order.Table);

            MenuItem menuItem = GetReceiptItem(order.Item);
            if (menuItem == null) //unknown food or drink
            {
                return;
            }

            Receipt receipt = null;
            foreach (Receipt rcpt in receipts)
            {
                if (rcpt.Table == order.Table)
                {
                    receipt = rcpt;
                    break;
                }
            }
            if (receipt == null)
            {
                receipt = new Receipt();
                receipt.Table = order.Table;
                receipts.Add(receipt);
            }

            Item item = null;
            foreach (Item itm in receipt.Items)
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
                receipt.Items.Add(item);
            }

            item.Quantity += 1;
            item.TotalPrice = item.Quantity * menuItem.Price;
        }

        public Receipt GetReceipt(Payment payment)
        {
            Console.WriteLine("get receipt");
            Console.WriteLine(payment.Table);

            //find, delete and send
            Receipt receipt = null;
            foreach (Receipt rcpt in receipts)
            {
                if (rcpt.Table == payment.Table)
                {
                    receipt = rcpt;
                    receipts.Remove(rcpt);
                    break;
                }
            }
            if (receipt == null)
            {
                receipt = new Receipt();
                receipt.Table = payment.Table;
            }
            receipt.Date = DateTime.Now;

            return receipt;
        }
    }
}
