using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;

namespace FlowDiagrams.Controls
{
    public class FlowItem : FrameworkElement
    {
        private Rect mainRect;
        private Rect[] dockingSpots;
        private double spotSize = 5; // Adjust the size of the docking spots as needed

        private Point _lastMousePosition;
        private bool _isDragging;

        public FlowItem()
        {
            mainRect = new Rect(10 ,10, 50, 50);
            Init();
        }
        public FlowItem(System.Windows.Point point)
        {
            mainRect = new Rect(point.X, point.Y, 50, 50);
            Init();
        }
        private void Init()
        {
            dockingSpots = new Rect[4];

            _isDragging = false;
            IsHitTestVisible = true;

            // Mouse event handlers
            MouseLeftButtonDown += DockableRectangle_MouseLeftButtonDown;
            MouseLeftButtonUp += DockableRectangle_MouseLeftButtonUp;
            MouseMove += DockableRectangle_MouseMove;
        }
        private void DockableRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point mousePosition = e.GetPosition(this);

                // Check if the mouse is over any of the docking spots
                bool isMouseOverDockingSpot = false;
                foreach (var spot in dockingSpots)
                {
                    if (spot.Contains(mousePosition))
                    {
                        isMouseOverDockingSpot = true;
                        break;
                    }
                }

                if (!isMouseOverDockingSpot)
                {
                    double deltaX = mousePosition.X - _lastMousePosition.X;
                    double deltaY = mousePosition.Y - _lastMousePosition.Y;

                    // Update the position of the main rectangle
                    mainRect.X += deltaX;
                    mainRect.Y += deltaY;

                    // Update the docking spots
                    for (int i = 0; i < dockingSpots.Length; i++)
                    {
                        dockingSpots[i] = new Rect(dockingSpots[i].X + deltaX, dockingSpots[i].Y + deltaY, dockingSpots[i].Width, dockingSpots[i].Height);
                    }

                    _lastMousePosition = mousePosition;

                    // Invalidate the control to trigger a redraw
                    InvalidateVisual();
                }
                else
                {
                    _isDragging = false;
                    ReleaseMouseCapture();
                }
            }
        }

        private void DockableRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ReleaseMouseCapture();
        }

        private void DockableRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            _isDragging = true;
            CaptureMouse();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            // Draw the main rectangle
            drawingContext.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Black, 1), mainRect);

            // Calculate docking spots
            double halfSpotSize = spotSize / 2;

            dockingSpots[0] = new Rect(mainRect.TopLeft.X - halfSpotSize + mainRect.Width/2, mainRect.TopLeft.Y - halfSpotSize, spotSize, spotSize);
            dockingSpots[1] = new Rect(mainRect.TopRight.X - halfSpotSize, mainRect.TopRight.Y - halfSpotSize + mainRect.Height/2, spotSize, spotSize);
            dockingSpots[2] = new Rect(mainRect.BottomRight.X - halfSpotSize - mainRect.Width / 2, mainRect.BottomRight.Y - halfSpotSize, spotSize, spotSize);
            dockingSpots[3] = new Rect(mainRect.BottomLeft.X - halfSpotSize, mainRect.BottomLeft.Y - halfSpotSize - mainRect.Height/2, spotSize, spotSize);

            // Draw docking spots
            foreach (var spot in dockingSpots)
            {
                drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 1), spot);
            }
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            return new System.Windows.Size(200, 200); // Adjust the default size of the rectangle as needed
        }
    }
}
