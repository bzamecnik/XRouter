using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace XRouter.Gui
{
    class ConfigurationTreeItem
    {
        public string Name { get; private set; }

        public IConfigurationControl ConfigurationControl { get; private set; }

        private Func<IConfigurationControl> ControlFactory { get; set; }

        public ConfigurationTreeItem Parent { get; private set; }

        private List<ConfigurationTreeItem> InternalChildren { get; set; }
        public ReadOnlyCollection<ConfigurationTreeItem> Children { get; private set; }

        private ConfigurationManager ConfigManager { get; set; }

        public ConfigurationTreeItem(string name, Func<IConfigurationControl> controlFactory, ConfigurationTreeItem parent, ConfigurationManager configManager)
        {
            Name = name;
            ControlFactory = controlFactory;
            Parent = parent;
            ConfigManager = configManager;
            InternalChildren = new List<ConfigurationTreeItem>();
            Children = new ReadOnlyCollection<ConfigurationTreeItem>(InternalChildren);

            if (Parent != null) {
                Parent.InternalChildren.Add(this);
            }

            if (ControlFactory != null) {
                CreateConfigurationControl();
            }
        }

        private void CreateConfigurationControl()
        {
            ConfigurationControl = ControlFactory();
            ConfigurationControl.Initialize(ConfigManager, this);
        }

        public void SaveRecursively()
        {
            ConfigurationControl.Save();
            foreach (var child in Children) {
                child.SaveRecursively();
            }
        }
    }
}
