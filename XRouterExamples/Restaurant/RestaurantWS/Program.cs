using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouterWS
{
    class Program
    {
        static void Main(string[] args)
        {
            System.ServiceModel.ServiceHost objServiceHost = null;
            try
            {                
                objServiceHost = new System.ServiceModel.ServiceHost(typeof(Service));
                objServiceHost.Open();

                Console.WriteLine("Listening... <press enter to stop>");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(e.ToString());
                Console.ReadLine();
            }
            finally
            {
                try
                {
                    if (objServiceHost != null)
                        objServiceHost.Close();
                }
                catch
                { }
            }
        }
    }
}
