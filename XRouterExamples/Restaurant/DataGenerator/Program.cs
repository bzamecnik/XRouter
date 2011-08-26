using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XRouter.Examples.Restaurant.DataGenerator
{
    // identifiers are counted from 1

    class Program
    {
        static void Main(string[] args)
        {
            int tables = 10;
            int orders = 50;

            int maxFoodId = 9;
            int maxDrinkId = 5;

            string savePath = @"Data\Generated";

            if (args.Length == 5)
            {
                savePath = args[0];
                Int32.TryParse(args[1], out tables);
                Int32.TryParse(args[2], out orders);
                Int32.TryParse(args[3], out maxFoodId);
                Int32.TryParse(args[4], out maxDrinkId);
            }

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            DataGenerator gen = new DataGenerator()
            {
                MaxDrinkId = maxDrinkId,
                MaxFoodId = maxFoodId,
                OutputPath = savePath
            };

            Console.WriteLine("Generating payments...");
            gen.GeneratePayments(tables);
            Console.WriteLine("Payments have been generated");

            Console.WriteLine("Generating orders...");
            gen.GenerateOrders(orders, tables);
            Console.WriteLine("Orders have been generated");
        }
    }
}
