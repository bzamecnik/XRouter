using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Text;

namespace XRouter.Examples.Restaurant.DataGenerator
{
    class DataGenerator
    {
        public int MaxFoodId { get; set; }
        public int MaxDrinkId { get; set; }

        public string OutputPath { get; set; }

        Random random = new Random();

        public void GeneratePayments(int tableCount)
        {
            for (int table = 1; table <= tableCount; ++table)
            {
                XElement payment = new XElement("payment", new XElement("table", table));
                XDocument doc = new XDocument();
                doc.Add(payment);

                string fileName = string.Format("paymentForTable{0}.xml", table.ToString().PadLeft(2, '0'));
                doc.Save(Path.Combine(OutputPath, fileName));
            }
        }

        public void GenerateOrders(int orderCount, int tableCount)
        {
            for (int orderNumber = 1; orderNumber <= orderCount; ++orderNumber)
            {
                XElement order = new XElement("order");
                int tableId = random.Next(1, tableCount + 1);
                order.Add(new XElement("table", tableId));

                string ItemId;
                int maxId;
                if (random.Next(2) == 0)
                {
                    ItemId = "food";
                    maxId = MaxFoodId;
                }
                else
                {
                    ItemId = "drink";
                    maxId = MaxDrinkId;
                }
                ItemId += random.Next(1, maxId + 1).ToString().PadLeft(4, '0');
                order.Add(new XElement("item", ItemId));

                XDocument doc = new XDocument();
                doc.Add(order);

                string fileName = string.Format("order{0}.xml", orderNumber.ToString().PadLeft(5, '0'));
                doc.Save(Path.Combine(OutputPath, fileName));
            }
        }
    }
}
