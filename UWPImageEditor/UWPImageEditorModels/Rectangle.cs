using System;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;

namespace UWPImageEditor.UWPImageEditorModels
{
    class Rectangle : IDrawing
    {
        private float x;
        private float y;

        private float width;
        private float height;

        private Color drawingColor;
        private int drawingSize;

        public Rectangle(Color color, int size, Point point)
        {
            drawingColor = color;
            drawingSize = size;
            x = (float)point.X;
            y = (float)point.Y;
        }

        public void AddPoint(Point point)
        {
            width = (float)point.X - x;
            height = (float)point.Y - y;
        }

        public void Draw(CanvasDrawingSession graphics)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            var builder = new CanvasPathBuilder(device);
            var brush = new CanvasSolidColorBrush(graphics, drawingColor);

            graphics.DrawRectangle(x, y, width, height, brush, drawingSize);
        }
    }
}
