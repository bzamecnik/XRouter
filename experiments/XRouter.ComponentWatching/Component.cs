using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XRouter.ComponentWatching
{
    class Component
    {
        public object ComponentObject { get; private set; }

        private IComponentsDataStorage Storage { get; set; }

        public FrameworkElement Representation { get; private set; }

        public Point Location {
            get { return Storage.GetLocation(ComponentObject); }
            set {                
                Storage.SetLocation(ComponentObject, value);
                Canvas.SetLeft(Representation, Location.X);
                Canvas.SetTop(Representation, Location.Y);
            }
        }

        internal static bool IsComponent(object obj)
        {
            bool result = obj.GetType().GetCustomAttributes(typeof(WatchableComponentAttribute), true).Any();
            return result;
        }

        internal Component(object componentObject, IComponentsDataStorage storage)
        {
            ComponentObject = componentObject;
            Storage = storage;

            WatchableComponentAttribute componentAttribute = (WatchableComponentAttribute)componentObject.GetType().GetCustomAttributes(typeof(WatchableComponentAttribute), true).Single();
            
            if (componentObject is IRepresentationProvider) {
                IRepresentationProvider representationProvider = (IRepresentationProvider)componentObject;
                Representation = representationProvider.CreateRepresentation();
            } else {
                Representation = CreateDefaultRepresentation(componentAttribute);
            }
            Canvas.SetLeft(Representation, Location.X);
            Canvas.SetTop(Representation, Location.Y);
        }

        private FrameworkElement CreateDefaultRepresentation(WatchableComponentAttribute componentAttribute)
        {
            FrameworkElement result = new Border {                
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(5),
                Child = new TextBlock {
                    Text = componentAttribute.ComponentName,
                    FontSize = 14d,
                    FontWeight = FontWeights.Bold
                }                
            };
            return result;
        }
    }
}
