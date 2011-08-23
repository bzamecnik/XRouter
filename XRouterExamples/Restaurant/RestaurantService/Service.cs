using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XRouter.Examples.Restaurant.RestaurantService
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
            menu = XDocument.Load(@"Data\RestaurantMenu.xml");
        }
      
        public void SaveOrder(Order order)
        {
            MenuItem menuItem = GetReceiptItem(order.Item);
            if (menuItem == null) //unknown food or drink
            {
                return;
            }

            Receipt receipt = receipts.FirstOrDefault((r) =>r.Table == order.Table);
            if (receipt == null)
            {
                receipt = new Receipt() { Table = order.Table };
                receipts.Add(receipt);
            }

            Item item = receipt.Items.FirstOrDefault((i) => i.Name == menuItem.Name);
            if (item == null)
            {
                item = new Item()
                {
                    Name = menuItem.Name,
                    Quantity = 0,
                    TotalPrice = 0
                };
                receipt.Items.Add(item);
            }

            item.Quantity += 1;
            item.TotalPrice = item.Quantity * menuItem.Price;
        }

        public Receipt GetReceipt(Payment payment)
        {
            //find, delete and send
            Receipt receipt = receipts.FirstOrDefault((r) => r.Table == payment.Table);
            if (receipt != null)
            {
                receipts.Remove(receipt);
            } else
            {
                receipt = new Receipt() { Table = payment.Table };
            }
            receipt.Date = DateTime.Now.ToString();

            return receipt;
        }
    }
}
