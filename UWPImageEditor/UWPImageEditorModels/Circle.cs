using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;
using Windows.UI;
using Microsoft.Graphics.Canvas.Brushes;

namespace UWPImageEditor.UWPImageEditorModels
{
    class Circle : IDrawing
    {
        private float centerX;
        private float centerY;

        private float radiusX;
        private float radiusY;

        private Color drawingColor;
        private int drawingSize;

        public Circle(Color color, int size, Point point)
        {
            drawingColor = color;
            drawingSize = size;
            centerX = (float)point.X;
            centerY = (float)point.Y;
        }

        public void AddPoint(Point point)
        {
            radiusX = centerX - (float)point.X;
            radiusY = centerY - (float)point.Y;
        }

        public void Draw(CanvasDrawingSession graphics)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var builder = new CanvasPathBuilder(device);
            var brush = new CanvasSolidColorBrush(graphics, drawingColor);

            graphics.DrawEllipse(centerX, centerY, radiusX, radiusY, brush, drawingSize);
        }
    }
}
