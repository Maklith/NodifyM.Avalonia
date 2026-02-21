using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace NodifyM.Avalonia.Helpers
{
    internal static class DependencyObjectExtensions
    {
        public static T? GetParentOfType<T>(this Control child,string? name=null)
            where T : Control
        {
            Visual? current = child;
            
            while (true)
            {
                
            
                current = current.GetVisualParent();
                if (current == default)
                {
                    return default;
                }

                if (current is T control)
                {
                    if (name!=null&&control.Name!=name)
                    {
                        continue;
                    }
                    return control;
                }
            } 

            
        }
        public static T? GetChildOfType<T>(this Control control, string? name = null)
            where T : Control
        {
            var queue = new Queue<Control>();
            queue.Enqueue(control);

            while (queue.Count > 0)
            {
                var currentControl = queue.Dequeue();

                if (string.IsNullOrEmpty(name) && currentControl is T targetControl)
                {
                    return targetControl;
                }

                foreach (var child in currentControl.GetVisualChildren())
                {
                    var childControl = child as Control;
                    if (childControl != null)
                    {
                        if (string.IsNullOrEmpty(name) || childControl.Name == name)
                        {
                            if (childControl is T targetChild)
                            {
                                return targetChild;
                            }
                        }

                        queue.Enqueue(childControl);
                    }
                }
            }

            return null;
        }

        public static T? GetVisualAt<T>(this Control control,Point anchor)
            where T : Control
        {
            var visualAt = control.GetVisualAt(anchor);
            if (visualAt == null)
                return null;
            else
                return ((Control)visualAt).GetParentOfType<T>();
        }
        
    }
}