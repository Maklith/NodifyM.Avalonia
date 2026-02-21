using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events;
public delegate void NodifyAutoPanningEventHandler(object sender, NodifyAutoPanningEventArgs e);
public class NodifyAutoPanningEventArgs : RoutedEventArgs{
    public BaseNode Node { get; set; }

    public NodifyAutoPanningEventArgs(RoutedEvent? routedEvent, BaseNode node) : base(routedEvent)
    {
        Node = node;
       
    }
}
