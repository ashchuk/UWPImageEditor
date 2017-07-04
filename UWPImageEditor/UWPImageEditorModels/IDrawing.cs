using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace UWPImageEditor.UWPImageEditorModels
{
    interface IDrawing
    {
        void AddPoint(Point point);
        void Draw(CanvasDrawingSession graphics);
    }
}
