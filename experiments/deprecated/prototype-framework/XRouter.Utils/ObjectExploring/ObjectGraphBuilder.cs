using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Utils.DataStructures;
using XRouter.Utils.DesignTemplates.NavigatedExploring;

namespace XRouter.Utils.ObjectExploring
{
    public class ObjectGraphBuilder
    {
        public static OrientedGraph<object> CreateGraph(object root)
        {
            return CreateGraph(new object[] { root });
        }

        public static OrientedGraph<object> CreateGraph(IEnumerable<object> roots, Func<ObjectInfo, bool> objectFilter = null, Func<ObjectLinkInfo, bool> referenceFilter = null)
        {
            Context context = new Context {
                Graph = new OrientedGraph<object>(),
                ObjectFilter = (objectFilter != null) ? objectFilter : _ => true,
                ReferenceFilter = (referenceFilter != null) ? referenceFilter : _ => true
            };            
            foreach (object root in roots) {
                ObjectNavigator navigator = new ObjectNavigator(root);
                VisitObject(navigator, context);
            }
            return context.Graph;
        }

        private static void VisitObject(Navigator<ObjectInfo, ObjectLinkInfo> navigator, Context context)
        {
            object currentObject = navigator.Location.Object;
            if (context.Graph.Nodes.Contains(currentObject)) {
                return;
            }
            if (!context.ObjectFilter(navigator.Location)) {
                return;
            }
            context.Graph.Nodes.Add(currentObject);

            foreach (var forwardLink in navigator.GetForwardLinks()) {
                if (!context.ReferenceFilter(forwardLink.Info)) {
                    continue;
                }
                if (forwardLink.TargetLocation.RealKind == TypeKind.Null) {
                    continue;
                }
                var targetNavigator = forwardLink.Navigate();                
                context.Graph.AddEdge(currentObject, targetNavigator.Location.Object);
                VisitObject(targetNavigator, context);
            }
        }

        private class Context
        {
            public Func<ObjectInfo, bool> ObjectFilter { get; set; }
            public Func<ObjectLinkInfo, bool> ReferenceFilter { get; set; }
            public OrientedGraph<object> Graph { get; set; }
        }
    }
}
