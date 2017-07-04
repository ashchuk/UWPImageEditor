using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace UWPImageEditor.UWPImageEditorModels
{
    class Arrow : IDrawing
    {
        private Vector2 _arrowHead;
        private Vector2 _arrowTail;

        public Color drawingColor;
        public int drawingSize;

        public Arrow(Color color, int size, Point tailPosition)
        {
            _arrowTail = new Vector2((float)tailPosition.X, (float)tailPosition.Y);
            _arrowHead = new Vector2((float)tailPosition.X, (float)tailPosition.Y);

            drawingColor = color;
            drawingSize = size;
        }

        public void AddPoint(Point point)
        {
            _arrowHead.X = (float)point.X;
            _arrowHead.Y = (float)point.Y;
        }

        public void Draw(CanvasDrawingSession graphics)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();

            var builder = new CanvasPathBuilder(device);

            var originalW = _arrowHead.X - _arrowTail.X;
            var originalH = _arrowHead.Y - _arrowTail.Y;
            var originalL = Math.Sqrt(originalW * originalW + originalH * originalH);

            var newL = originalL - Math.Min(20, originalL / 2);
            var newW = originalW * newL / originalL;
            var newH = originalH * newL / originalL;

            var newX = _arrowTail.X + (float)newW;
            var newY = _arrowTail.Y + (float)newH;

            var normalK = -originalW / originalH;

            var cosA = 1 / (float)Math.Sqrt(1 + normalK * normalK);
            var step = 20 * cosA;

            var x1 = newX - step;
            var y1 = normalK * (x1 - newX) + newY;
            var arrowLeftSholder = new Vector2(x1, y1);

            var x2 = newX + step;
            var y2 = normalK * (x2 - newX) + newY;
            var arrowRightSholder = new Vector2(x2, y2);

            builder.BeginFigure(_arrowTail);
            builder.AddLine(_arrowHead);
            builder.EndFigure(CanvasFigureLoop.Open);

            builder.BeginFigure(arrowLeftSholder);
            builder.AddLine(_arrowHead);
            builder.AddLine(arrowRightSholder);
            builder.EndFigure(CanvasFigureLoop.Open);

            var arrow = CanvasGeometry.CreatePath(builder);
            graphics.DrawGeometry(arrow, drawingColor, drawingSize, new CanvasStrokeStyle
            {
                TransformBehavior = CanvasStrokeTransformBehavior.Fixed,
                StartCap = CanvasCapStyle.Triangle,
                EndCap = CanvasCapStyle.Triangle,
                LineJoin = CanvasLineJoin.Miter
            });
        }
    }
}
