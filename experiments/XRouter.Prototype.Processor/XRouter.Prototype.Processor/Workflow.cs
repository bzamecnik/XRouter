using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Reflection;

namespace XRouter.Prototype.Processor
{
    class Workflow
    {
        private Int32 version = -1;

        private Node rootNode = null;

        private Dictionary<Int32, Node> nodes = new Dictionary<Int32, Node>();

        private Dictionary<Int32, Edge> edges = new Dictionary<Int32, Edge>();

        // předpokládá validní konfiguraci
        public Workflow(XDocument xWorkflow)
        {
            this.Deserialize(xWorkflow);
        }

        // zahrnuje pouze deserializaci objektoveho modelu z XML
        private void Deserialize(XDocument xWorkflow)
        {
            // sejme verzi 
            this.version = Convert.ToInt32(xWorkflow.Root.Attribute(XName.Get("version")).Value);

            // vytvoří instance edges     
            foreach (XElement xNode in xWorkflow.XPathSelectElements("//edge"))
            {
                Int32 id = Convert.ToInt32(xNode.Attribute(XName.Get("id")).Value);
                Int32 from = Convert.ToInt32(xNode.Attribute(XName.Get("from")).Value);
                Int32 to = Convert.ToInt32(xNode.Attribute(XName.Get("to")).Value);
                
                Edge edge = new Edge();
                edge.Id = id;
                edge.From = from;
                edge.To = to;
                this.edges.Add(id, edge);

            }

            // vytvoří instance nodes
            Int32 j = 0;
            foreach (XElement xNode in xWorkflow.XPathSelectElements("//node"))
            {
                Int32 id = Convert.ToInt32(xNode.Attribute(XName.Get("id")).Value);
                
                Node node = new Node();
                node.Id = id;

                // vybere child edges
                List<Edge> childEdges = new List<Edge>();
                foreach (KeyValuePair<Int32, Edge> item in this.edges)
                {
                    if (item.Value.From == id)
                    {
                        childEdges.Add(item.Value);
                    }
                }
                Int32[] steps = new Int32[childEdges.Count];
                for (Int32 i = 0; i < childEdges.Count; i++)
                {
                    steps[i] = childEdges[i].Id;
                }

                // select name
                String name = xNode.Attribute(XName.Get("name")).Value;
                node.Name = name;

                // select type
                INodeFunction nodeFunc = null;
                String type = xNode.Attribute(XName.Get("type")).Value;
                switch (type)
                {
                    case "cbr":
                        {
                            nodeFunc = DeserializeCBR(name, id, steps, xNode);
                            break;
                        }
                    case "terminator":
                        {
                            nodeFunc = DeserializeTerminator(name, id, steps, xNode);
                            break;
                        }
                    default:
                        {
                            nodeFunc = DeserializeAction(steps, xNode);
                            break;
                        }
                }
                node.Function = nodeFunc;

                if (j == 0)
                {
                    this.rootNode = node;
                }
                j++;

                this.nodes.Add(id, node);
            }
        }

        // z XML vytvoti instanci CBR
        private INodeFunction DeserializeCBR(String name, Int32 nodeId, Int32[] steps, XElement xNode)
        {
            List<String> tests = new List<String>();
            foreach (XElement xPredicate in xNode.XPathSelectElements(String.Format("//node[@id='{0}']/predicate", nodeId)))
            {
                tests.Add(xPredicate.Attribute(XName.Get("test")).Value);
            }
            
            String[] arrTests = tests.ToArray();
            System.Diagnostics.Debug.Assert(arrTests.Length >= 2, "Pocet testu CBR musi byt alespon 2.");
            System.Diagnostics.Debug.Assert(tests[arrTests.Length - 1] == "default", "Posledni test CBR musi byt default.");
            CBR cbr = new CBR(name, arrTests, steps);

            return cbr;
        }

        // z XML vytvoti instanci Terminatoru
        private INodeFunction DeserializeTerminator(String name, Int32 nodeId, Int32[] steps, XElement xNode)
        {
            System.Diagnostics.Debug.Assert(steps.Length == 0, "Terminator nema nasledovaniky.");
            XElement xSync = xNode.XPathSelectElement(String.Format("//node[@id='{0}']/sync", nodeId));
            System.Diagnostics.Debug.Assert(xSync != null, "Element sync je pro termintor povinny.");

            Boolean sync = Convert.ToBoolean(xSync.Attribute(XName.Get("value")).Value);
            String xpath = null;
            if (sync)
            {
                xpath = xSync.Attribute(XName.Get("xpath")).Value;
            }

            Terminator terminator = new Terminator(name, sync, xpath);

            return terminator;
        }

        // z XML vytvoti instanci Akce
        private INodeFunction DeserializeAction(Int32[] steps, XElement xNode)
        {
            String name = xNode.Attribute(XName.Get("name")).Value;
            String type = xNode.Attribute(XName.Get("type")).Value;
            INodeFunction obj = (INodeFunction)Assembly.GetExecutingAssembly().CreateInstance(type);

            Action action = new Action(obj, name, steps[0]);
            return action;
        }

        public void Initialize()
        {
            foreach (KeyValuePair<Int32, Node> node in this.nodes)
            {
                if (node.Value.Function != null)
                {
                    node.Value.Function.Initialize();
                }
            }
        }

        public INodeFunction GetNext(Int32 step)
        {
            if (step == -1)
            {
                return this.rootNode.Function;
            }
            if (step == -2)
            {
                return null;
            }

            Int32 to = this.edges[step].To;
            return this.nodes[to].Function;
        }
    }
}
