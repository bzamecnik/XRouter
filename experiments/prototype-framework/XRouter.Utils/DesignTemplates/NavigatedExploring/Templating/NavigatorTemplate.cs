using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DesignTemplates.NavigatedExploring.Templating
{
    public abstract class NavigatorTemplate<TLocation, TLinkInfo>
    {
        protected internal TLocation CurrentLocation { get; internal set; }

        internal abstract IEnumerable<LinkTarget<TLocation, TLinkInfo>> GetForwardLinkTargets();
    }
}