using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace UWPImageEditor.UWPImageEditorModels
{
    public class Stroke : IDrawing
    {
        private List<Point> _points;

        private Color drawingColor;
        private int drawingSize;

        public Stroke(Color color, int size)
        {
            if (_points == null)
                _points = new List<Point>();
            drawingColor = color;
            drawingSize = size;
        }

        public void AddPoint(Point point) => _points.Add(point);

        public void Draw(CanvasDrawingSession graphics)
        {
            if (_points == null || _points.Count == 0)
                return;

            var brush = new CanvasSolidColorBrush(graphics, drawingColor);
            if (_points.Count == 1)
            {
                graphics.DrawLine((float)_points[0].X, (float)_points[0].Y, (float)_points[0].X, (float)_points[0].Y, brush, drawingSize);
                return;
            }
            var style = new CanvasStrokeStyle()
            {
                DashCap = CanvasCapStyle.Round,
                StartCap = CanvasCapStyle.Round,
                EndCap = CanvasCapStyle.Round
            };
            for (int i = 0; i < _points.Count - 1; ++i)
                graphics.DrawLine((float)_points[i].X, (float)_points[i].Y,
                                  (float)_points[i + 1].X, (float)_points[i + 1].Y,
                                  brush, drawingSize, style);
        }
    }
}
