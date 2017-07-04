using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;

namespace UWPImageEditor.UWPImageEditorModels
{
    class ClipboardBorder : IDrawing
    {
        private float x;
        private float y;

        private float width;
        private float height;

        private Color drawingColor = Colors.Black;
        private int drawingSize = 2;

        public ClipboardBorder(Point point)
        {
            x = (float)point.X;
            y = (float)point.Y;
        }

        public Rect ToRect()
        {
            if (this.width < 0)
            {
                this.x += this.width;
                this.width = System.Math.Abs(this.width);
            }
            if (this.height < 0)
            {
                this.y += this.height;
                this.height = System.Math.Abs(this.height);
            }
            return new Rect(this.x, this.y, this.width, this.height);
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
            var strokeStyle = new CanvasStrokeStyle() {DashStyle = CanvasDashStyle.Dash, DashOffset = 5f};

            graphics.DrawRectangle(x, y, width, height, brush, drawingSize, strokeStyle);
        }
    }
}
