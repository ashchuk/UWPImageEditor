using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWPImageEditor.Controls
{
    public static class VisualHelper
    {
        public static T FindVisualChildInsideFrame<T>(DependencyObject depObj) where T : DependencyObject
        {
            var frame = depObj as Frame;
            if (frame == null)
                frame = FindVisualChild<Frame>(depObj);

            if (frame != null && frame.Content is Page)
                return FindVisualChild<T>(frame.Content as Page);

            return null;
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null)
                        return childItem;
                }
            }
            return null;
        }

        public static T FindVisualChildByName<T>(FrameworkElement depObj, string name) where T : FrameworkElement
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i) as FrameworkElement;
                    if (child != null && child is T && child.Name == name)
                        return (T)child;

                    T childItem = FindVisualChildByName<T>(child, name);
                    if (childItem != null)
                        return childItem;
                }
            }
            return null;
        }
    }
}
