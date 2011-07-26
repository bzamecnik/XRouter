using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Common.MessageFlowConfig;
using XRouter.Processor.BuiltInActions;
using System.Xml.Linq;
using XRouter.Common;
using XRouter.Common.Xrm;

namespace XRouter.Processor.MessageFlowBuilding
{
    public class MessageFlowBuilder
    {
        public static TerminatorNodeConfiguration CreateTerminator(string name)
        {
            return new TerminatorNodeConfiguration() { Name = name, IsReturningOutput = false };
        }

        public static ActionNodeConfiguration CreateSender(string name, string inputMessage, string outputEndpoint, NodeConfiguration nextNode)
        {
            return new ActionNodeConfiguration() {
                Name = name, NextNode = nextNode, Actions = {
                    new ActionConfiguration() {
                        PluginTypeFullName = typeof(SendMessageAction).FullName,
                        PluginConfiguration = new SerializableXDocument(XDocument.Parse("<config input='"+inputMessage+"' output='"+outputEndpoint+"' />"))
                    }
                }
            };
        }

        public static ActionNodeConfiguration CreateTransformation(string name, string xslt, string inputMessage, string outputMessage, NodeConfiguration nextNode)
        {
            return new ActionNodeConfiguration() {
                Name = name, NextNode = nextNode, Actions = { 
                    new ActionConfiguration() { 
                        PluginTypeFullName = typeof(XsltTransformationAction).FullName,
                        PluginConfiguration = new SerializableXDocument(XDocument.Parse("<config xslt='"+xslt+"' input='"+inputMessage+"' output='"+outputMessage+"' />"))
                    }
                }
            };
        }

        public static CbrNodeConfiguration CreateCbr(string name, string testedMessage, NodeConfiguration defaultTarget, params CbrCase[] cases)
        {
            var testedMessageSelection = new TokenSelection("/token/messages/message[@name='" + testedMessage + "']/*[1]");
            var result = new CbrNodeConfiguration() { Name = name, TestedSelection = testedMessageSelection, DefaultTarget = defaultTarget };
            foreach (CbrCase cbrCase in cases) {
                result.Branches.Add(new XrmUri("//item[@name='" + cbrCase.Schematron + "']"), cbrCase.TargetNode);
            }
            return result;
        }
    }
}
