using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Restaurant.RestaurantDataCreation
{
    class Program
    {
        static void Main(string[] args)
        {
            int tables = 10;
            int orders = 50;

            int maxFoodID = 9;
            int maxDrinkID = 5;

            string savePath = @"..\..\..\Data/Generated\";

            Random random = new Random();

            Console.WriteLine("Generating orders...");
            for (int orderNumber = 1; orderNumber <= orders; ++orderNumber)
            {
                XDocument doc = new XDocument();

                XElement order = new XElement("order");
                order.Add(new XElement("table", random.Next(1, tables)));

                string ItemId;
                if (random.Next(0, 1) == 0)
                {
                    ItemId = "food" + random.Next(1, maxFoodID).ToString().PadLeft(4, '0');
                }
                else
                {
                    ItemId = "drink" + random.Next(1, maxDrinkID).ToString().PadLeft(4, '0');
                }
                order.Add(new XElement("item", ItemId));

                doc.Add(order);
                doc.Save(savePath + "order" + orderNumber.ToString().PadLeft(5, '0') + ".xml");
            }
            Console.WriteLine("Orders have been generated");

            Console.WriteLine("Generating payments...");
            for (int table = 1; table <= tables; ++table)
            {
                XDocument doc = new XDocument();

                XElement payment = new XElement("payment", new XElement("table", table));

                doc.Add(payment);
                doc.Save(savePath + "paymentForTable" + table.ToString().PadLeft(2, '0') + ".xml");
            }
            Console.WriteLine("Payments hace been generated");

            Console.WriteLine("Press a key...");
            Console.ReadKey();
        }
    }
}
