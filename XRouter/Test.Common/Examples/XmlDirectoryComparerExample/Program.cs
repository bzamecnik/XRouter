using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Test.Common;

namespace XRouter.Test.Common
{
    class Program
    {
        static void Main(string[] args)
        {            
            string correctRoot = @"..\..\Data\Correct";
            string resultRoot = @"..\..\Data\Result2";

            XmlDirectoryComparer comparer = new XmlDirectoryComparer(correctRoot, resultRoot);
            comparer.Compare();

            Console.ReadKey();
        }
    }
}
