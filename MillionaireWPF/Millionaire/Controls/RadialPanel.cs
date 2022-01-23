using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Millionaire.Controls
{
    internal class RadialPanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
            {
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            var angle = Math.PI / 2;
            var incrementalAngularSpace = (360.0 / Children.Count) * (Math.PI / 180);

            var radiusX = finalSize.Width / 2.4;
            var radiusY = finalSize.Height / 2.4;

            foreach (UIElement elem in Children)
            {
                //Calculate the point on the circle for the element
                var childPoint = new Point(Math.Cos(angle) * radiusX, -Math.Sin(angle) * radiusY);

                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                var actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));

                //Calculate the new _angle for the next element
                angle -= incrementalAngularSpace;
            }

            return finalSize;
        }

    }
}
