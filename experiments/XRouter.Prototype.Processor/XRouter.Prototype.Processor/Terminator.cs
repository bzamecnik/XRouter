using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;

namespace XRouter.Prototype.Processor
{
    class Terminator : INodeFunction
    {
        private String name = null;

        private Boolean sync;

        private String xpath = null;
              
        public Terminator(String name, Boolean sync, String xpath)
        {
            this.name = name;
            this.sync = sync;
            this.xpath = xpath;
        }

        public void Initialize()
        {
            Logger.LogInfo("Init Terminator." + name);

            
            Logger.LogInfo("Init Terminator." + name + " done!");
        }

        public void Evaluate(Token token)
        {
            Logger.LogInfo("Terminator." + name);

            if (sync)
            {
                XElement xEle = token.Content.XPathSelectElement(this.xpath);
                Logger.LogInfo("Terminator." + name + " odesilam na GW element:" + xEle.ToString());
            }

            token.Step = -2;

            Logger.LogInfo("Terminator." + name + " done!");
        }
    }
}
