using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace UWPImageEditor.UWPImageEditorModels
{
    class Clipboard : IDrawing
    {
        private int x;
        private int y;

        private CanvasBitmap _clipboard;

        public Clipboard(CanvasBitmap clipboard)
        {
            _clipboard = clipboard;
        }

        public void AddPoint(Point point)
        {
            x = (int)point.X;
            y = (int)point.Y;
        }

        public void Draw(CanvasDrawingSession graphics)
        {
            graphics.DrawImage(_clipboard, new Rect()
            {
                Height = _clipboard.Size.Height,
                Width = _clipboard.Size.Width,
                X = x,
                Y = y
            });

        }
    }
}
