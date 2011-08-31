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
using System.Collections.ObjectModel;

namespace XRouter.Gui.CommonControls
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    public partial class ListEditor : UserControl
    {
        private Func<FrameworkElement> ItemFactory { get; set; }

        public event Action<FrameworkElement> ItemAdded = delegate { };
        public event Action<FrameworkElement> ItemRemoved = delegate { };
        public event Action<FrameworkElement> ItemSelected = delegate { };

        private List<FrameworkElement> internalItems = new List<FrameworkElement>();
        public ReadOnlyCollection<FrameworkElement> Items { get; private set; }

        public ListEditor()
        {
            InitializeComponent();
            Items = new ReadOnlyCollection<FrameworkElement>(internalItems);
        }

        public void Initialize(Func<FrameworkElement> itemFactory)
        {
            ItemFactory = itemFactory;   
        }

        public void AddItem(FrameworkElement item)
        {
            internalItems.Add(item);

            Button uiRemove = new Button {
                Content = new Image {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/XRouter.GUI;component/Resources/delete.png")),
                    Width = 12,
                    Margin = new Thickness(1)
                },
                Style = (Style)FindResource(ToolBar.ButtonStyleKey),
                Margin = new Thickness(2, 0, 0, 0)
            };
            uiRemove.Click += delegate {
                internalItems.Remove(item);
                ListViewItem rowToRemove = uiItems.Items.OfType<ListViewItem>().First(i => i.Tag == item);
                uiItems.Items.Remove(rowToRemove);
                ItemRemoved(item);
            };
            Grid.SetColumn(uiRemove, 1);

            ListViewItem row = new ListViewItem {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Tag = item,
                Content = new Grid {
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    },
                    Children = {
                        item,
                        uiRemove
                    }
                },
                Margin = new Thickness(2, 2, 2, 2)
            };
            uiItems.Items.Add(row);

            item.GotFocus += delegate {
                uiItems.SelectedItem = row;
            };
        }

        public void SelectItem(FrameworkElement item)
        {
            if (item == null) {
                uiItems.SelectedIndex = -1;
                return;
            }

            ListViewItem rowToSelect = uiItems.Items.OfType<ListViewItem>().FirstOrDefault(i => i.Tag == item);
            if (rowToSelect != null) {
                uiItems.SelectedIndex = uiItems.Items.IndexOf(rowToSelect);
            }
        }

        private void uiAdd_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement item = ItemFactory();
            if (item == null) {
                return;
            }
            AddItem(item);
            ItemAdded(item);
            uiItems.SelectedIndex = uiItems.Items.Count - 1;
        }

        private void uiItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((uiItems.SelectedItem != null) && (uiItems.SelectedItem is ListViewItem)) {
                ItemSelected((FrameworkElement)((ListViewItem)uiItems.SelectedItem).Tag);
            } else {
                ItemSelected(null);
            }
        }
    }
}
