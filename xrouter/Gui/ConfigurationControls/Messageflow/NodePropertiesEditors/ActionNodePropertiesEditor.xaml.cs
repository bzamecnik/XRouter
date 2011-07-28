using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XRouter.Common.MessageFlowConfig;
using ObjectConfigurator;
using XRouter.Common;
using System.Threading.Tasks;
using System.Threading;
using XRouter.Gui.CommonControls;
using System.Xml.Linq;
using XRouter.Common.Utils;
using XRouter.Gui.Utils;

namespace XRouter.Gui.ConfigurationControls.Messageflow.NodePropertiesEditors
{
    /// <summary>
    /// Interaction logic for ActionNodePropertiesEditor.xaml
    /// </summary>
    public partial class ActionNodePropertiesEditor : UserControl
    {
        private ActionNodeConfiguration node;
        private NodeSelectionManager nodeSelectionManager;

        private ActionConfiguration activeAction;
        private ConfigurationEditor activeConfigurationEditor;

        internal ActionNodePropertiesEditor(ActionNodeConfiguration node, NodeSelectionManager nodeSelectionManager)
        {
            InitializeComponent();
            this.node = node;
            this.nodeSelectionManager = nodeSelectionManager;

            uiName.Text = node.Name;

            #region Prepare next node selector
            uiNextNodeSelector.Initialize(nodeSelectionManager, node, () => node.NextNode, delegate(NodeConfiguration nextNode) {
                node.NextNode = nextNode;
            });
            #endregion

            #region Prepare actions editing
            uiActionsListEditor.Initialize(AddAction);
            uiActionsListEditor.ItemRemoved += RemoveAction;
            uiActionsListEditor.ItemSelected += SetActiveAction;

            uiActionConfigurationRegion.Visibility = Visibility.Collapsed;
            foreach (ActionConfiguration action in node.Actions) {
                uiActionsListEditor.AddItem(CreateActionRepresentation(action));
            }
            uiActionsListEditor.SelectItem(uiActionsListEditor.Items.FirstOrDefault());
            #endregion
        }

        #region Name editing

        private void uiName_LostFocus(object sender, RoutedEventArgs e)
        {
            node.Name = uiName.Text;
        }

        private void uiName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                node.Name = uiName.Text;
            }
            if (e.Key == Key.Escape) {
                uiName.Text = node.Name;
            }
        }

        #endregion

        #region Actions editing

        private FrameworkElement AddAction()
        {
            ActionConfiguration action = new ActionConfiguration();
            action.Configuration = new SerializableXDocument(new XDocument());
            action.Configuration.XDocument.Add(new XElement(XName.Get("objectConfig")));
            action.PluginTypeFullName = typeof(XRouter.Processor.BuiltInActions.SendMessageAction).FullName;
            action.ConfigurationMetadata = new ClassMetadata(typeof(XRouter.Processor.BuiltInActions.SendMessageAction));
            node.Actions.Add(action);

            FrameworkElement actionRepresentation = CreateActionRepresentation(action);
            return actionRepresentation;
        }

        private FrameworkElement CreateActionRepresentation(ActionConfiguration action)
        {
            string[] actionTypeNames = new string[] { 
                typeof(XRouter.Processor.BuiltInActions.SendMessageAction).FullName,
                typeof(XRouter.Processor.BuiltInActions.XsltTransformationAction).FullName
            };
            ComboBox uiActionType = new ComboBox {
                Tag = action,
                Margin = new Thickness(10, 3, 5, 3),
                IsEditable = false,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            foreach (string actionTypeName in actionTypeNames) {
                uiActionType.Items.Add(new ComboBoxItem {
                    Tag = actionTypeName,
                    Content = actionTypeName.Substring(actionTypeName.LastIndexOf('.') + 1)
                });
            }
            uiActionType.SelectedIndex = actionTypeNames.ToList().IndexOf(action.PluginTypeFullName);
            uiActionType.SelectionChanged += delegate {
                string selectedActionTypeName = (string)(((ComboBoxItem)uiActionType.SelectedItem).Tag);
                action.PluginTypeFullName = selectedActionTypeName;
                action.ConfigurationMetadata = new ClassMetadata(TypeUtils.GetType(selectedActionTypeName, "XRouter.Processor.dll"));
                SetActiveAction(uiActionType);
            };
            Grid.SetColumn(uiActionType, 1);

            var uiAction = new Grid {
                Tag = action,
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Children = {
                    new Image {
                        Source = new BitmapImage(new Uri("pack://application:,,,/XRouter.Gui;component/Resources/Actions-tool-animator-icon.png")),
                        Height = 18,
                        Margin = new Thickness(8, 0, 0, 0)
                    },
                    uiActionType
                }
            };

            return uiAction;
        }

        private void RemoveAction(FrameworkElement uiAction)
        {
            ActionConfiguration action = (ActionConfiguration)uiAction.Tag;
            node.Actions.Remove(action);
        }

        private void SetActiveAction(FrameworkElement uiAction)
        {
            if (uiAction == null) {
                activeAction = null;
                uiActionConfigurationRegion.Visibility = Visibility.Collapsed;
                return;
            }
            
            activeAction = (ActionConfiguration)uiAction.Tag;
            uiActionConfigurationRegion.Visibility = Visibility.Visible;

            activeConfigurationEditor = Configurator.CreateEditor(activeAction.ConfigurationMetadata);
            activeConfigurationEditor.LoadConfiguration(activeAction.Configuration.XDocument);
            activeConfigurationEditor.ConfigurationChanged += delegate {
                activeAction.Configuration = new SerializableXDocument(activeConfigurationEditor.SaveConfiguration());
            };
            uiActionConfigurationContainer.Child = activeConfigurationEditor;
        }

        #endregion
    }
}
