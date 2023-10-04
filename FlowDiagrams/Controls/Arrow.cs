using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace FlowDiagrams.Controls
{
    
    public class Arrow : FrameworkElement
    {
        private Point startPoint;
        private bool isDragging = false;

        public Arrow()
        {

        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(this);
                isDragging = true;
                this.CaptureMouse();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                this.ReleaseMouseCapture();
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point endPoint = e.GetPosition(this);

                double offsetX = endPoint.X - startPoint.X;
                double offsetY = endPoint.Y - startPoint.Y;

                Canvas.SetLeft(this, Canvas.GetLeft(this) + offsetX);
                Canvas.SetTop(this, Canvas.GetTop(this) + offsetY);

                startPoint = endPoint;
            }
        }
    }
}
