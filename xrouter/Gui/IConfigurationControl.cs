using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XRouter.Common.ComponentInterfaces;
using XRouter.Common;

namespace XRouter.Gui
{
	public interface IConfigurationControl
	{
        bool IsDirty { get; }

		void Initialize(ApplicationConfiguration appConfig, IBrokerServiceForManagement brokerService, ConfigurationTree configTreeNode);

        void Save();
		void Clear();
	}
}
