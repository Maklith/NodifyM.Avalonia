// Author: liaom
// SolutionName: Kitopia
// ProjectName: NodifyM.Avalonia
// FileName:BaseNodeGroup.axaml.cs
// Date: 2025/09/26 20:09
// FileEffect:

using Avalonia;
using Avalonia.Media;
using NodifyM.Avalonia.Events;

namespace NodifyM.Avalonia.Controls;

public class NodeGroup : BaseNode
{
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<NodeGroup, object>(
        nameof(Header));

    public static readonly StyledProperty<SolidColorBrush> HeaderBrushProperty =
        AvaloniaProperty.Register<NodeGroup, SolidColorBrush>(
            nameof(HeaderBrush));

    public static readonly StyledProperty<Size> SizeProperty = AvaloniaProperty.Register<NodeGroup, Size>(
        nameof(Size));

    public static readonly StyledProperty<bool> CanResizeProperty = AvaloniaProperty.Register<NodeGroup, bool>(
        nameof(CanResize));


    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public SolidColorBrush HeaderBrush
    {
        get => GetValue(HeaderBrushProperty);
        set => SetValue(HeaderBrushProperty, value);
    }

    public bool CanResize
    {
        get => GetValue(CanResizeProperty);
        set => SetValue(CanResizeProperty, value);
    }

    public Size Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override void OnSelectChanged(NodeSelectEventArgs e)
    {
        base.OnSelectChanged(e);
    }
}