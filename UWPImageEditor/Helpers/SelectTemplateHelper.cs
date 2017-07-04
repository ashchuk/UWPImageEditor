using UWPImageEditor.UWPImageEditorModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPImageEditor.Helpers
{
    public class SelectTemplateHelper : DataTemplateSelector
    {
        public DataTemplate ArrowTemplate { get; set; }
        public DataTemplate PenTemplate { get; set; }
        public DataTemplate CircleTemplate { get; set; }
        public DataTemplate RectangleTemplate { get; set; }
        public DataTemplate PasteTemplate { get; set; }
        public DataTemplate SelectionTemplate { get; set; }
        public DataTemplate MovingTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object obj, DependencyObject container)
        {
            if ((DrawingMode)obj == DrawingMode.Lines)
                return PenTemplate;
            if ((DrawingMode)obj == DrawingMode.Arrows)
                return ArrowTemplate;
            if ((DrawingMode)obj == DrawingMode.Circles)
                return CircleTemplate;
            if ((DrawingMode)obj == DrawingMode.Rectangles)
                return RectangleTemplate;
            if ((DrawingMode)obj == DrawingMode.Selection)
                return SelectionTemplate;
            if ((DrawingMode)obj == DrawingMode.Moving)
                return MovingTemplate;

            return PenTemplate;
        }
    }
}
