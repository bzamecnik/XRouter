using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Utils.DesignTemplates.NavigatedExploring.Templating;

namespace XRouter.Utils.DesignTemplates.NavigatedExploring
{
    public class NavigatorLink<TLocation, TLinkInfo>
    {
        private Func<NavigatorTemplate<TLocation, TLinkInfo>> templateInstanceFactory;

        public TLocation SourceLocation { get; private set; }
        public TLocation TargetLocation { get; private set; }

        public TLinkInfo Info { get; private set; }

        internal NavigatorLink(Func<NavigatorTemplate<TLocation, TLinkInfo>> templateInstanceFactory,
            TLocation source, TLocation target, TLinkInfo info)
        {
            this.templateInstanceFactory = templateInstanceFactory;

            SourceLocation = source;
            TargetLocation = target;
            Info = info;
        }

        public Navigator<TLocation, TLinkInfo> Navigate()
        {
            var result = new Navigator<TLocation, TLinkInfo>(templateInstanceFactory, TargetLocation);
            result.BackLink = this;
            return result;
        }
    }
}
