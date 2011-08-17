using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common;
using XRouter.Manager;

namespace XRouter.Gui
{
	interface IConfigurationControl
	{
        event Action ConfigChanged;

        void Initialize(ConfigurationManager configManager, ConfigurationTreeItem configTreeItem);

        void Save();
	}
}
