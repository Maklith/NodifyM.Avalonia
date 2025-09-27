using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;

namespace NodifyM.Avalonia.Controls;

public class BaseNode : ContentControl
{
    public static readonly AvaloniaProperty<Point> LocationProperty =
        AvaloniaProperty.Register<BaseNode, Point>(nameof(Location));

    public static readonly RoutedEvent LocationChangedEvent =
        RoutedEvent.Register<NodeLocationEventArgs>(nameof(LocationChanged), RoutingStrategies.Bubble,
            typeof(BaseNode));

    public static readonly RoutedEvent IsSelectChangedEvent =
        RoutedEvent.Register<NodeLocationEventArgs>(nameof(IsSelectChanged), RoutingStrategies.Bubble,
            typeof(BaseNode));

    public static readonly AvaloniaProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<BaseNode, bool>(nameof(IsSelected));


    private NodifyEditor? _editor;


    private double _startOffsetX;
    private double _startOffsetY;

    /// <summary>
    /// 标记是否先启动了拖动
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// 记录上一次鼠标位置
    /// </summary>
    private Point lastMousePosition;

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            NodeSelectChanged(new NodeSelectEventArgs(this, IsSelected, IsSelectChangedEvent));
        }
    }

    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set
        {
            SetValue(LocationProperty, value);
            NodeLocationChanged(new NodeLocationEventArgs(Location, this, LocationChangedEvent, true));
        }
    }

    public event NodeLocationEventHandler LocationChanged
    {
        add => AddHandler(LocationChangedEvent, value);
        remove => RemoveHandler(LocationChangedEvent, value);
    }

    public event NodeSelectEventHandler IsSelectChanged
    {
        add => AddHandler(IsSelectChangedEvent, value);
        remove => RemoveHandler(IsSelectChangedEvent, value);
    }


    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_editor is null)
        {
            return;
        }

        if (e.Handled)
        {
            return;
        }

        _editor.StopNodeDrag(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_editor is null)
        {
            return;
        }

        if (e.Handled)
        {
            return;
        }

        _editor.StartNodeDrag(e, this);
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _editor = this.GetParentOfType<NodifyEditor>();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (_editor is null)
        {
            return;
        }

        NodeLocationChanged(new NodeLocationEventArgs(Location, this, LocationChangedEvent, true));
        _editor?.ClearAlignmentLine();
    }

    private void NodeLocationChanged(NodeLocationEventArgs e)
    {
        OnLocationChanged(e);
        RaiseEvent(e);
    }

    private void NodeSelectChanged(NodeSelectEventArgs e)
    {
        OnSelectChanged(e);
        RaiseEvent(e);
    }

    protected virtual void OnSelectChanged(NodeSelectEventArgs e)
    {
    }

    protected virtual void OnLocationChanged(NodeLocationEventArgs e)
    {
    }
}