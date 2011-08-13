using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Utils.DesignTemplates.NavigatedExploring.Templating;
using XRouter.Utils.DesignTemplates.NavigatedExploring;

namespace XRouter.Utils.ObjectExploring
{
    public class ObjectNavigator : Navigator<ObjectInfo, ObjectLinkInfo>
    {
        public ObjectNavigator(object obj)
            : base(() => new ObjectNavigatorTemplateInstance(), new ObjectInfo(obj))
        {
        }
    }
}
