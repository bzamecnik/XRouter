using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Controls;
using ObjectConfigurator.ValueValidators;
using System.Windows;
using ObjectConfigurator.ItemTypes;
using System.Windows.Media;

namespace ObjectConfigurator.ValueEditors
{
    class CollectionValueEditor : ValueEditor
    {
        private CollectionItemType collectionValueType;

        private StackPanel uiItems;
        private Image uiError;
        private Button uiAdd;

        private XElement xDefaultElementValue;

        public CollectionValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            collectionValueType = (CollectionItemType)ValueType;

            xDefaultElementValue = new XElement(XName.Get("defaultValue"));
            collectionValueType.ElementType.WriteDefaultValueToXElement(xDefaultElementValue);

            uiItems = new StackPanel {
            };

            uiAdd = new Button {
                Content = new StackPanel {
                    Orientation = Orientation.Horizontal,
                    Children = {
                        new Image {
                            Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/ObjectConfigurator;component/Resources/plus.png")),
                            Width = 12,
                            Margin = new Thickness(5, 0, 0, 0)
                        },
                        new TextBlock {
                            Margin = new Thickness(5, 0, 5, 0),
                            VerticalAlignment = VerticalAlignment.Center,
                            Text = "Add"
                        }
                    }
                },
                Margin = new Thickness(2, 4, 2, 2),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            uiAdd.Click += uiAdd_Click;

            uiError = new Image {
                Margin = new Thickness(10, 0, 0, 0),
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/ObjectConfigurator;component/Resources/error.png")),
                Width = 20,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(uiError, 1);

            uiItems.Children.Add(new Grid {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                },
                Children = { uiAdd, uiError }
            });

            Representation = new Border {
                Background = Brushes.GhostWhite,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(5),
                Child = uiItems
            };

            CheckErrors();
        }

        private void uiAdd_Click(object sender, RoutedEventArgs e)
        {
            AddListItemForElement(xDefaultElementValue);
            CheckErrors();
            RaiseValueChanged();
        }

        public override bool WriteToXElement(XElement target)
        {
            var items = uiItems.Children.OfType<Border>().Where(i => i.Tag is ValueEditor).ToArray();
            foreach (Border item in items) {
                ValueEditor elementEditor = (ValueEditor)item.Tag;
                XElement xElement = new XElement(CollectionItemType.XName_CollectionElement);
                if (elementEditor.WriteToXElement(xElement)) {
                    target.Add(xElement);
                }
            }
            return true;
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = SerializedDefaultValue;
            }

            var oldItems = uiItems.Children.OfType<Border>().Where(i => i.Tag is ValueEditor).ToArray();
            foreach (var item in oldItems) {
                uiItems.Children.Remove(item);
            }

            foreach (XElement xElement in source.Elements(CollectionItemType.XName_CollectionElement)) {
                AddListItemForElement(xElement);
            }
            CheckErrors();
        }

        private void AddListItemForElement(XElement xElement)
        {
            ValueEditor elementEditor = ValueEditor.CreateEditor(collectionValueType.ElementType, new ValueValidatorAttribute[0], xDefaultElementValue);
            elementEditor.ReadFromXElement(xElement);
            elementEditor.ValueChanged += delegate { RaiseValueChanged(); };

            Button uiRemove = new Button {
                Content = new Image {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/ObjectConfigurator;component/Resources/delete.png")),
                    Width = 12,
                    Margin = new Thickness(1)
                },
                Margin = new Thickness(2, 0, 0, 0)
            };
            uiRemove.Click += delegate {
                Border itemToRemove = uiItems.Children.OfType<Border>().First(i => i.Tag == elementEditor);
                uiItems.Children.Remove(itemToRemove);
                CheckErrors();
                RaiseValueChanged();
            };
            Grid.SetColumn(uiRemove, 1);

            Border item = new Border {
                Tag = elementEditor,
                Child = new Grid {
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    },
                    Children = {
                        elementEditor.Representation,
                        uiRemove
                    }
                },
                Margin = new Thickness(2, 2, 2, 2)
            };
            uiItems.Children.Insert(uiItems.Children.Count - 1, item);
        }

        private void CheckErrors()
        {
            var items = uiItems.Children.OfType<Border>().Where(i => i.Tag is ValueEditor).ToArray();
            object[] elements = new object[items.Length];

            string errorDescription;
            if (IsValid(elements, out errorDescription)) {
                uiError.Visibility = Visibility.Collapsed;
            } else {
                uiError.ToolTip = errorDescription;
                uiError.Visibility = Visibility.Visible;
            }
        }
    }
}
