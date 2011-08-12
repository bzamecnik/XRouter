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
	public interface IConfigurationControl
	{
        bool IsDirty { get; }

        void Initialize(ApplicationConfiguration appConfig, IConsoleServer consoleServer, ConfigurationTree configTreeNode);

        void Save();
		void Clear();
	}
}
