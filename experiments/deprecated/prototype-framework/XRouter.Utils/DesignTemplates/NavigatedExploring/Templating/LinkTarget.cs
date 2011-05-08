using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRouter.Utils.DesignTemplates.NavigatedExploring.Templating
{
    public sealed class LinkTarget<TLocation, TLinkInfo>
    {
        public TLocation Target { get; private set; }
        public TLinkInfo Info { get; private set; }

        public LinkTarget(TLocation target, TLinkInfo info)
        {
            Target = target;
            Info = info;
        }
    }
}
