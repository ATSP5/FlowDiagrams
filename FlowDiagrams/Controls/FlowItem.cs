using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace FlowDiagrams.Controls
{
    public class FlowItem : FrameworkElement
    {
        private Rect mainRect;
        private Rect[] dockingSpots;
        private Line[] _lines;
        private double spotSize = 5; // Adjust the size of the docking spots as needed

        private Point _lastMousePosition;
        private bool _isDragging;
        private bool _isDrawingLine;
        private int _selectedDockSpot;

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
            _lines = new Line[4];
            _isDragging = false;
            _isDrawingLine = false;
            IsHitTestVisible = true;

            // Mouse event handlers
            MouseLeftButtonDown += DockableRectangle_MouseLeftButtonDown;
            MouseLeftButtonUp += DockableRectangle_MouseLeftButtonUp;
            MouseMove += DockableRectangle_MouseMove;
        }
        private int IsSelectedDockSpot(Point mousePosition)
        {
            int isMouseOverDockingSpot = -1;
            var counter = 0;
            foreach (var spot in dockingSpots)
            {
                if (spot.Contains(mousePosition))
                {
                    isMouseOverDockingSpot = counter;
                    break;
                }
                counter++;
            }
            return isMouseOverDockingSpot;
        }

        private void DockableRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point mousePosition = e.GetPosition(this);

                
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
                //Update position of all available lines
                for(int i =0; i< _lines.Length; i++)
                {
                    if(_lines[i] != null)
                    {
                        _lines[i].X1 = dockingSpots[i].X + deltaX;
                        _lines[i].Y1 = dockingSpots[i].Y + deltaY;
                    }
                }

                 _lastMousePosition = mousePosition;

                 // Invalidate the control to trigger a redraw
                 InvalidateVisual();             
            }
            else if(_isDrawingLine)
            {
                var mousePosition = e.GetPosition(this);
                _lines[_selectedDockSpot].X2 = mousePosition.X;
                _lines[_selectedDockSpot].Y2 = mousePosition.Y;

                // Redraw the docking spots and the lines
                InvalidateVisual();
            }
        }
  
        private void DockableRectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            _isDrawingLine = false;
            ReleaseMouseCapture();
        }

        private void DockableRectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _lastMousePosition = e.GetPosition(this);
            _selectedDockSpot = IsSelectedDockSpot(_lastMousePosition);
            _isDragging = _selectedDockSpot <0? true:false;
            _isDrawingLine = _selectedDockSpot >= 0 ? true : false;
            if(_isDrawingLine)
            {
                _lines[_selectedDockSpot] = new Line
                {
                    X1 = _lastMousePosition.X,
                    Y1 = _lastMousePosition.Y,
                    X2 = _lastMousePosition.X,
                    Y2 = _lastMousePosition.Y
                };
            }
            CaptureMouse();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            foreach(var line in _lines)
             if(line != null)
                drawingContext.DrawLine(new Pen(Brushes.Black, 1),
                    new Point(line.X1, line.Y1),
                    new Point(line.X2, line.Y2));
            

                // Draw the main rectangle
                drawingContext.DrawRectangle(Brushes.LightBlue, new Pen(Brushes.Black, 1), mainRect);

                // Calculate docking spots
                double halfSpotSize = spotSize / 2;

                dockingSpots[0] = new Rect(mainRect.TopLeft.X - halfSpotSize + mainRect.Width / 2, mainRect.TopLeft.Y - halfSpotSize, spotSize, spotSize);
                dockingSpots[1] = new Rect(mainRect.TopRight.X - halfSpotSize, mainRect.TopRight.Y - halfSpotSize + mainRect.Height / 2, spotSize, spotSize);
                dockingSpots[2] = new Rect(mainRect.BottomRight.X - halfSpotSize - mainRect.Width / 2, mainRect.BottomRight.Y - halfSpotSize, spotSize, spotSize);
                dockingSpots[3] = new Rect(mainRect.BottomLeft.X - halfSpotSize, mainRect.BottomLeft.Y - halfSpotSize - mainRect.Height / 2, spotSize, spotSize);

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
