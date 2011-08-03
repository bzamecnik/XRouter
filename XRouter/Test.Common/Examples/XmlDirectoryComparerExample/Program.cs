using System;

namespace XRouter.Test.Common
{
    class Program
    {
        static void Main(string[] args)
        {            
            string correctRoot = @"..\..\Data\Correct";
            string resultRoot = @"..\..\Data\Result2";

            XmlDirectoryComparer.Equals(correctRoot, resultRoot, true);

            Console.ReadKey();
        }
    }
}
