using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ObjectConfigurator.ValueValidators;
using ObjectConfigurator.ItemTypes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace ObjectConfigurator.ValueEditors
{
    class DictionaryValueEditor : ValueEditor
    {
        private DictionaryItemType dictionaryValueType;

        private StackPanel uiItems;
        private Image uiError;
        private Button uiAdd;

        private XElement xDefaultKey;
        private XElement xDefaultValue;

        public DictionaryValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            dictionaryValueType = (DictionaryItemType)ValueType;

            xDefaultKey = new XElement(XName.Get("defaultValue"));
            dictionaryValueType.KeyType.WriteDefaultValueToXElement(xDefaultKey);
            xDefaultValue = new XElement(XName.Get("defaultValue"));
            dictionaryValueType.ValueType.WriteDefaultValueToXElement(xDefaultValue);

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
            AddListItemForPair(xDefaultKey, xDefaultValue);
            CheckErrors();
            RaiseValueChanged();
        }

        public override bool WriteToXElement(XElement target)
        {
            var items = uiItems.Children.OfType<Border>().Where(i => i.Tag is KeyValuePair<ValueEditor, ValueEditor>).ToArray();
            foreach (Border item in items) {
                KeyValuePair<ValueEditor, ValueEditor> editorsPair = (KeyValuePair<ValueEditor, ValueEditor>)item.Tag;
                ValueEditor keyEditor = editorsPair.Key;
                ValueEditor valueEditor = editorsPair.Value;

                XElement xPair = new XElement(DictionaryItemType.XName_PairElement);
                XElement xKey = new XElement(DictionaryItemType.XName_KeyElement);
                XElement xValue = new XElement(DictionaryItemType.XName_ValueElement);
                if ((keyEditor.WriteToXElement(xKey)) && (valueEditor.WriteToXElement(xValue))) {
                    xPair.Add(xKey);
                    xPair.Add(xValue);
                    target.Add(xPair);
                }
            }
            return true;
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = SerializedDefaultValue;
            }

            var oldItems = uiItems.Children.OfType<Border>().Where(i => i.Tag is KeyValuePair<ValueEditor, ValueEditor>).ToArray();
            foreach (var item in oldItems) {
                uiItems.Children.Remove(item);
            }

            foreach (XElement xPair in source.Elements(DictionaryItemType.XName_PairElement)) {
                XElement xKey = xPair.Element(DictionaryItemType.XName_KeyElement);
                XElement xValue = xPair.Element(DictionaryItemType.XName_ValueElement);
                AddListItemForPair(xKey, xValue);
            }
            CheckErrors();
        }

        private void AddListItemForPair(XElement xKey, XElement xValue)
        {
            ValueEditor keyEditor = ValueEditor.CreateEditor(dictionaryValueType.KeyType, new ValueValidatorAttribute[0], xDefaultKey);
            keyEditor.ReadFromXElement(xKey);
            keyEditor.ValueChanged += delegate { RaiseValueChanged(); };

            ValueEditor valueEditor = ValueEditor.CreateEditor(dictionaryValueType.ValueType, new ValueValidatorAttribute[0], xDefaultValue);
            valueEditor.ReadFromXElement(xValue);
            valueEditor.ValueChanged += delegate { RaiseValueChanged(); };

            var editorsPair = new KeyValuePair<ValueEditor, ValueEditor>(keyEditor, valueEditor);

            Button uiRemove = new Button {
                Content = new Image {
                    Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/ObjectConfigurator;component/Resources/delete.png")),
                    Width = 12,
                    Margin = new Thickness(1)
                },
                Margin = new Thickness(2, 0, 0, 0)
            };
            uiRemove.Click += delegate {
                Border itemToRemove = uiItems.Children.OfType<Border>().First(i => editorsPair.Equals(i.Tag));
                uiItems.Children.Remove(itemToRemove);
                CheckErrors();
                RaiseValueChanged();
            };
            Grid.SetColumn(uiRemove, 2);

            Grid.SetColumn(valueEditor.Representation, 1);
            valueEditor.Representation.Margin = new Thickness(5, 0, 0, 0);
            Border item = new Border {
                Tag = editorsPair,
                Child = new Grid {
                    ColumnDefinitions = {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    },
                    Children = {
                        keyEditor.Representation,
                        valueEditor.Representation,
                        uiRemove
                    }
                },
                Margin = new Thickness(2, 2, 2, 2)
            };
            uiItems.Children.Insert(uiItems.Children.Count - 1, item);
        }

        private void CheckErrors()
        {
            var items = uiItems.Children.OfType<Border>().Where(i => i.Tag is KeyValuePair<ValueEditor, ValueEditor>).ToArray();
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
