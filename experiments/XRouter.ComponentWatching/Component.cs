using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using XRouter.Utils.Wpf;

namespace XRouter.ComponentWatching
{
    class Component
    {
        public string Name { get; private set; }

        public object ComponentObject { get; private set; }

        private IComponentsDataStorage Storage { get; set; }

        public FrameworkElement RepresentationContainer { get; private set; }
        
        public Point Location {
            get { return Storage.GetLocation(Name); }
            set {
                Storage.SetLocation(Name, value);
                Canvas.SetLeft(RepresentationContainer, Location.X);
                Canvas.SetTop(RepresentationContainer, Location.Y);
            }
        }

        public event Action VisualBoundsChanged = delegate { };
        public Rect VisualBounds {
            get {
                double left = Canvas.GetLeft(RepresentationContainer);
                double top = Canvas.GetTop(RepresentationContainer);
                Size size = RepresentationContainer.GetSize();
                Rect result = new Rect(left, top, size.Width, size.Height);
                return result;
            }
        }

        internal static bool IsComponent(object obj)
        {
            Type type = obj.GetType();
            bool hasWatchableComponentInterface = obj is IWatchableComponent;
            if (!hasWatchableComponentInterface) {
                bool hasWatchableComponentAttribute = type.GetCustomAttributes(typeof(WatchableComponentAttribute), true).Any();
                if (!hasWatchableComponentAttribute) {
                    return false;
                }
            }
            return true;
        }

        internal Component(object componentObject, IComponentsDataStorage storage)
        {
            ComponentObject = componentObject;
            Storage = storage;

            Type componentType = componentObject.GetType();

            FrameworkElement representation = null;
            if (componentObject is IWatchableComponent) {
                IWatchableComponent watchableComponent = (IWatchableComponent)componentObject;
                Name = watchableComponent.ComponentName;
                representation = watchableComponent.CreateRepresentation();
            } else {
                WatchableComponentAttribute componentAttribute = (WatchableComponentAttribute)componentType.GetCustomAttributes(typeof(WatchableComponentAttribute), true).Single();
                Name = componentAttribute.ComponentName;
            }

            RepresentationContainer = CreateRepresentationContainer(representation, Name);
            Canvas.SetLeft(RepresentationContainer, Location.X);
            Canvas.SetTop(RepresentationContainer, Location.Y);

            representation.SizeChanged += delegate {
                VisualBoundsChanged();
            };
        }

        private FrameworkElement CreateRepresentationContainer(FrameworkElement representation, string componentName)
        {
            double cornerRadius = 5;

            dragArea = new Border {
                Background = Brushes.LightSteelBlue,
                CornerRadius = new CornerRadius(cornerRadius, cornerRadius, 0, 0),
                Padding = new Thickness(5, 3, 5, 3),
                Child = new TextBlock {
                    Text = componentName,
                    FontSize = 14d,
                    FontWeight = FontWeights.Bold
                }
            };
            dragArea.MouseLeftButtonDown += DragArea_MouseLeftButtonDown;
            dragArea.MouseLeftButtonUp += DragArea_MouseLeftButtonUp;
            dragArea.PreviewMouseMove += DragArea_PreviewMouseMove;
            dragArea.MouseRightButtonDown += delegate { CancelDragging(); };

            FrameworkElement result = new Border {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(cornerRadius),
                Child = new StackPanel {
                    Children = {
                        dragArea,
                        new Border {
                            Background = Brushes.LightSkyBlue,
                            CornerRadius = new CornerRadius(0, 0, cornerRadius, cornerRadius),
                            Padding = new Thickness(3),
                            Child = representation
                        }
                    }
                }
            };
            return result;
        }

        #region Dragging
        private FrameworkElement dragArea;
        private bool isDragging = false;
        private Point dragMouseLocationOnNode;
        private Point currentDragLocation;

        void DragArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragMouseLocationOnNode = e.GetPosition(RepresentationContainer);
            isDragging = true;
            dragArea.CaptureMouse();
            currentDragLocation = Location;
        }

        void DragArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging) {
                return;
            }
            if (dragArea.IsMouseCaptured) {
                dragArea.ReleaseMouseCapture();
            }
            isDragging = false;
            Location = currentDragLocation;
        }

        void DragArea_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) {
                return;
            }
            Vector moveDelta = e.GetPosition(RepresentationContainer) - dragMouseLocationOnNode;
            currentDragLocation += moveDelta;
            Canvas.SetLeft(RepresentationContainer, currentDragLocation.X);
            Canvas.SetTop(RepresentationContainer, currentDragLocation.Y);
            VisualBoundsChanged();
        }

        private void CancelDragging()
        {
            if (!isDragging) {
                return;
            }
            if (dragArea.IsMouseCaptured) {
                dragArea.ReleaseMouseCapture();
            }
            isDragging = false;
            Canvas.SetLeft(RepresentationContainer, Location.X);
            Canvas.SetTop(RepresentationContainer, Location.Y);
            VisualBoundsChanged();
        }
        #endregion
    }
}
