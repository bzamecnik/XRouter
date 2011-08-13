using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XRouter.Utils.Wpf
{
    public static class WpfExtensions
    {
        public static Size GetSize(this FrameworkElement frameworkElement)
        {
            Size result = new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight);

            if (result == new Size(0, 0)) {
                frameworkElement.Measure(new Size(double.MaxValue, double.MaxValue));
                result = frameworkElement.DesiredSize;
            }
            return result;
        }

        public static System.Drawing.Color ToWinFormColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        public static Color ToWpfColor(this System.Drawing.Color color)
        {
            return Color.FromRgb(color.R, color.G, color.B);
        }
    }
}
