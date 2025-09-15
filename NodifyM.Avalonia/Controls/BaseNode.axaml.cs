using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
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
        set => SetValue(IsSelectedProperty, value);
    }

    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    public event NodeLocationEventHandler LocationChanged
    {
        add => AddHandler(LocationChangedEvent, value);
        remove => RemoveHandler(LocationChangedEvent, value);
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

        if (!isDragging) return;
        // 停止拖动
        isDragging = false;
        e.Handled = true;
        // 停止计时器
        _editor.ClearAlignmentLine();

        // var currentPoint = e.GetCurrentPoint(this);
        //  Debug.WriteLine($"停止拖动坐标X:{OffsetX} Y:{OffsetY}");
        RaiseEvent(new NodeLocationEventArgs(Location, this, LocationChangedEvent, true));
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

        if (e.Source is Control control)
        {
            if (control is ComboBox)
            {
                return;
            }

            if (control.GetParentOfType<ComboBox>() is not null)
            {
                return;
            }
        }

        _editor.SelectItem(this);
        if (!e.GetCurrentPoint(this)
                .Properties.IsLeftButtonPressed) return;
        e.GetCurrentPoint(this).Pointer.Capture(this);
        // 启动拖动
        isDragging = true;
        // 记录当前坐标
        var relativeTo = ((Visual)this.GetLogicalParent()).GetVisualParent();
        lastMousePosition = e.GetPosition((Visual)relativeTo);
        // Debug.WriteLine($"记录当前坐标X:{lastMousePosition.X} Y:{lastMousePosition.Y}");
        _startOffsetX = Location.X;
        _startOffsetY = Location.Y;
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_editor is null)
        {
            return;
        }

        if (e.Handled)
        {
            return;
        }

        if (!e.GetCurrentPoint(((Visual)this.GetLogicalParent()).GetVisualParent())
                .Properties
                .IsLeftButtonPressed) return;

        // 如果没有启动拖动，则不执行
        if (!isDragging) return;

        var currentMousePosition = e.GetPosition(((Visual)this.GetLogicalParent()).GetVisualParent());
        var offset = currentMousePosition - lastMousePosition;

        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
        {
            _editor.ClearAlignmentLine();
            Location = new Point((offset.X + _startOffsetX), offset.Y + _startOffsetY);
        }
        else
            Location = _editor.TryAlignNode(this,
                new Point((offset.X + _startOffsetX), offset.Y + _startOffsetY));

        RaiseEvent(new NodeLocationEventArgs(Location, this, LocationChangedEvent));
        e.Handled = true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _editor = this.GetParentOfType<NodifyEditor>();
        if (_editor != null) _editor.NodifyAutoPanning += NodifyAutoPanningEvent;
    }

    private void NodifyAutoPanningEvent(object sender, NodifyAutoPanningEventArgs e)
    {
        if (e.Node != this)
        {
            return;
        }

        RaiseEvent(new NodeLocationEventArgs(Location, this, LocationChangedEvent));
    }


    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (_editor is null)
        {
            return;
        }

        RaiseEvent(new NodeLocationEventArgs(Location, this, LocationChangedEvent, true));
        _editor?.ClearAlignmentLine();
    }
}