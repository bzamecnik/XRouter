using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Windows;
using ObjectConfigurator.ValueValidators;
using ObjectConfigurator.ItemTypes;

namespace ObjectConfigurator.ValueEditors
{
    class BasicValueEditor : ValueEditor
    {
        private static readonly string InvalidValueError = "Value is invalid.";

        private BasicItemType basicValueType;

        private TextBox uiText;
        private Image uiError;

        public BasicValueEditor(ItemType valueType, IEnumerable<ValueValidatorAttribute> validators, XElement serializedDefaultValue)
            : base(valueType, validators, serializedDefaultValue)
        {
            basicValueType = (BasicItemType)ValueType;

            Grid grid = new Grid {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
                }
            };
            Representation = grid;

            uiText = new TextBox();
            uiText.TextChanged += uiText_TextChanged;
            Grid.SetColumn(uiText, 0);
            grid.Children.Add(uiText);

            uiError = new Image {
                Margin = new Thickness(5, 0, 0, 0),
                Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/ObjectConfigurator;component/Resources/error.png")),
                Width = 18
            };
            Grid.SetColumn(uiError, 1);
            grid.Children.Add(uiError);

            CheckError();
        }

        private void uiText_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckError();
        }

        private void CheckError()
        {
            object value;
            bool isValid = TryGetValue(out value);
            if (isValid) {
                string errorDescription;
                isValid = IsValid(value, out errorDescription);
                if (isValid) {
                    uiError.Visibility = Visibility.Collapsed;
                } else {
                    uiError.ToolTip = errorDescription;
                    uiError.Visibility = Visibility.Visible;
                }
            } else {
                uiError.ToolTip = InvalidValueError;
                uiError.Visibility = Visibility.Visible;
            }
        }

        public override bool WriteToXElement(XElement target)
        {
            object value;
            if (TryGetValue(out value)) {
                basicValueType.WriteToXElement(target, value);
                return true;
            } else {
                return false;
            }
        }

        public override void ReadFromXElement(XElement source)
        {
            if (source == null) {
                source = SerializedDefaultValue;
            }

            object value = basicValueType.ReadFromXElement(source);
            uiText.Text = basicValueType.ValueToString(value) ?? string.Empty;
        }

        private bool TryGetValue(out object value)
        {
            try {
                value = basicValueType.ParseValue(uiText.Text);
                return true;
            } catch {
                value = null;
                return false;
            }
        }
    }
}
