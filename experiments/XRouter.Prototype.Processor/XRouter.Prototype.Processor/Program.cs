using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XRouter.Prototype.Processor
{
    /// <summary>
    /// Implementace obsahuje experimentalni (ale funkcni) kod pro ozkouseni pruchodu tokenu 
    /// grafem workflow. 
    /// 
    /// Workflow se definuje v souboru workflow.xml.
    /// 
    /// V adresari Schematron jsou pouzite Schematron dokumenty.
    /// 
    /// V adresari XML jsou pouzite XML zpravy.
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            XDocument xWorkflow = XDocument.Load("../../workflow.xml");
            Workflow workflow = new Workflow(xWorkflow);
            workflow.Initialize();

            Console.WriteLine();

            Token token = new Token();
            token.Step = -1;
            token.Content = XDocument.Load("../../XML/message.xml", LoadOptions.SetLineInfo);

            while (true)
            {
                INodeFunction func = workflow.GetNext(token.Step);
                if (func == null)
                {
                    break;
                }
                func.Evaluate(token);              
            }

            Console.ReadLine();
        }
    }
}
