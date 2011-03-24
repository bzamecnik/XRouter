using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CBR_prototype
{
    /// <summary>
    /// Assembly obsahuje ukazku implementace komponenty Content Based Router, resp. vsechny 
    /// podstatne veci, ktere potrebujeme pro implementaci smerovace.
    /// 
    /// Konfiguruje se to pomoci souboru CBR_prototype.xml. 
    /// 
    /// Vstupni XML zpravou je message.xml a dale tu mame 4 predikaty (p1.xml ... p4.xml), 
    /// jejichz semantika je popsana v komentarich primo v nich. 
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // načte nějaký xml obsah (zprávu)
               // String messagePath = args[0];  
                String messagePath = @"..\..\message.xml";
                XDocument xMessage = XDocument.Load(messagePath, LoadOptions.SetLineInfo);
                    
                // připraví CBR
                CBR cbr = CBR.Deserialize(@"..\..\CBR_prototype.xml");
                cbr.Compile();

                // provede routování
                String id = cbr.Route(xMessage);
                Console.WriteLine(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
            }
            Console.ReadLine();
        }
    }
}
