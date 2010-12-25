using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XRouter.Utils.DesignTemplates.NavigatedExploring.Templating;

namespace XRouter.Utils.DesignTemplates.NavigatedExploring
{
    public class Navigator<TLocation, TLinkInfo>
    {
        private Func<NavigatorTemplate<TLocation, TLinkInfo>> templateInstanceFactory;
        private NavigatorTemplate<TLocation, TLinkInfo> templateInstance;

        public TLocation Location { get { return templateInstance.CurrentLocation; } }

        public NavigatorLink<TLocation, TLinkInfo> BackLink { get; internal set; }

        public Navigator(Func<NavigatorTemplate<TLocation, TLinkInfo>> templateInstanceFactory, TLocation location)
        {
            this.templateInstanceFactory = templateInstanceFactory;
            this.templateInstance = templateInstanceFactory();
            this.templateInstance.CurrentLocation = location;
        }

        public IEnumerable<NavigatorLink<TLocation, TLinkInfo>> GetForwardLinks()
        {
            foreach (var linkTarget in templateInstance.GetForwardLinkTargets()) {
                var result = new NavigatorLink<TLocation, TLinkInfo>(
                    templateInstanceFactory,
                    templateInstance.CurrentLocation,
                    linkTarget.Target,
                    linkTarget.Info);

                yield return result;
            }
        }
    }
}
