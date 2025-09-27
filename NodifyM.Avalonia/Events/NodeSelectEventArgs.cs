// Author: liaom
// SolutionName: Kitopia
// ProjectName: NodifyM.Avalonia
// FileName:NodeSelectEventArgs.cs
// Date: 2025/09/27 14:09
// FileEffect:

using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events;

public delegate void NodeSelectEventHandler(object sender, NodeSelectEventArgs e);

public class NodeSelectEventArgs : RoutedEventArgs
{
    public NodeSelectEventArgs(BaseNode sender, bool isSelected, RoutedEvent routedEvent)
    {
        Sender = sender;
        IsSelected = isSelected;
        RoutedEvent = routedEvent;
    }

    public BaseNode Sender { get; set; }
    public bool IsSelected { get; set; }
}