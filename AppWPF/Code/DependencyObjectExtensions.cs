using System.Windows;
using System.Windows.Media;

namespace AppWPF.Code
{
    public static class DependencyObjectExtensions
    {
        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
                return null;

            if (parent is T typedParent)
                return typedParent;

            return FindParent<T>(parent);
        }
    }
}