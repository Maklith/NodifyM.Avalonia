using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;

namespace NodifyM.Avalonia.Controls;

public class NodifyEditorMinimap : Control
{
    public static readonly StyledProperty<NodifyEditor?> EditorProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, NodifyEditor?>(nameof(Editor));

    public static readonly StyledProperty<IBrush> MapBackgroundProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(MapBackground),
            new SolidColorBrush(Color.FromArgb(190, 30, 30, 30)));

    public static readonly StyledProperty<IBrush> MapBorderBrushProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(MapBorderBrush),
            new SolidColorBrush(Color.FromArgb(220, 220, 220, 220)));

    public static readonly StyledProperty<IBrush> NodeBrushProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(NodeBrush),
            new SolidColorBrush(Color.FromArgb(170, 150, 190, 255)));

    public static readonly StyledProperty<IBrush> NodeBorderBrushProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(NodeBorderBrush),
            new SolidColorBrush(Color.FromArgb(220, 98, 140, 210)));

    public static readonly StyledProperty<IBrush> ViewportBrushProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(ViewportBrush),
            new SolidColorBrush(Color.FromArgb(70, 255, 255, 255)));

    public static readonly StyledProperty<IBrush> ViewportBorderBrushProperty =
        AvaloniaProperty.Register<NodifyEditorMinimap, IBrush>(nameof(ViewportBorderBrush),
            new SolidColorBrush(Color.FromArgb(220, 255, 255, 255)));

    private const double MapPadding = 8d;
    private NodifyEditor? _subscribedEditor;
    private bool _isDragging;

    static NodifyEditorMinimap()
    {
        AffectsRender<NodifyEditorMinimap>(
            MapBackgroundProperty,
            MapBorderBrushProperty,
            NodeBrushProperty,
            NodeBorderBrushProperty,
            ViewportBrushProperty,
            ViewportBorderBrushProperty);

        EditorProperty.Changed.Subscribe(args =>
        {
            if (args.Sender is NodifyEditorMinimap minimap)
            {
                minimap.AttachEditor(args.NewValue.Value);
            }
        });
    }

    public NodifyEditor? Editor
    {
        get => GetValue(EditorProperty);
        set => SetValue(EditorProperty, value);
    }

    public IBrush MapBackground
    {
        get => GetValue(MapBackgroundProperty);
        set => SetValue(MapBackgroundProperty, value);
    }

    public IBrush MapBorderBrush
    {
        get => GetValue(MapBorderBrushProperty);
        set => SetValue(MapBorderBrushProperty, value);
    }

    public IBrush NodeBrush
    {
        get => GetValue(NodeBrushProperty);
        set => SetValue(NodeBrushProperty, value);
    }

    public IBrush NodeBorderBrush
    {
        get => GetValue(NodeBorderBrushProperty);
        set => SetValue(NodeBorderBrushProperty, value);
    }

    public IBrush ViewportBrush
    {
        get => GetValue(ViewportBrushProperty);
        set => SetValue(ViewportBrushProperty, value);
    }

    public IBrush ViewportBorderBrush
    {
        get => GetValue(ViewportBorderBrushProperty);
        set => SetValue(ViewportBorderBrushProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachEditor(Editor);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        DetachEditor(_subscribedEditor);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isDragging = true;
        e.Pointer.Capture(this);
        PanViewportTo(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isDragging || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        PanViewportTo(e.GetPosition(this));
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _isDragging = false;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!TryGetMapGeometry(out var worldBounds, out var mapBounds, out var scale))
        {
            return;
        }

        var mapRect = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.FillRectangle(MapBackground, mapRect);
        context.DrawRectangle(null, new Pen(MapBorderBrush), mapRect.Deflate(0.5d));

        foreach (var node in GetVisibleNodes(Editor!))
        {
            var nodeRect = new Rect(node.Location, GetNodeSize(node));
            var mappedRect = WorldToMap(nodeRect, worldBounds, mapBounds, scale);
            if (mappedRect.Width <= 0 || mappedRect.Height <= 0)
            {
                continue;
            }

            context.FillRectangle(NodeBrush, mappedRect);
            context.DrawRectangle(null, new Pen(NodeBorderBrush), mappedRect.Deflate(0.5d));
        }

        var viewportWorld = GetViewportWorldRect(Editor!);
        var viewportRect = WorldToMap(viewportWorld, worldBounds, mapBounds, scale);
        context.FillRectangle(ViewportBrush, viewportRect);
        context.DrawRectangle(null, new Pen(ViewportBorderBrush, 1.2d), viewportRect.Deflate(0.6d));
    }

    private void AttachEditor(NodifyEditor? newEditor)
    {
        if (_subscribedEditor == newEditor)
        {
            return;
        }

        DetachEditor(_subscribedEditor);

        if (newEditor == null)
        {
            InvalidateVisual();
            return;
        }

        _subscribedEditor = newEditor;
        newEditor.PropertyChanged += OnEditorPropertyChanged;
        newEditor.SizeChanged += OnEditorSizeChanged;
        newEditor.LayoutUpdated += OnEditorLayoutUpdated;
        newEditor.AddHandler(BaseNode.LocationChangedEvent, OnEditorNodeLocationChanged);
        InvalidateVisual();
    }

    private void DetachEditor(NodifyEditor? editor)
    {
        if (editor == null)
        {
            return;
        }

        editor.PropertyChanged -= OnEditorPropertyChanged;
        editor.SizeChanged -= OnEditorSizeChanged;
        editor.LayoutUpdated -= OnEditorLayoutUpdated;
        editor.RemoveHandler(BaseNode.LocationChangedEvent, OnEditorNodeLocationChanged);

        if (_subscribedEditor == editor)
        {
            _subscribedEditor = null;
        }
    }

    private void OnEditorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == NodifyEditor.OffsetXProperty ||
            e.Property == NodifyEditor.OffsetYProperty ||
            e.Property == NodifyEditor.ZoomProperty ||
            e.Property == Visual.BoundsProperty)
        {
            InvalidateVisual();
        }
    }

    private void OnEditorSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        InvalidateVisual();
    }

    private void OnEditorLayoutUpdated(object? sender, EventArgs e)
    {
        InvalidateVisual();
    }

    private void OnEditorNodeLocationChanged(object? sender, NodeLocationEventArgs e)
    {
        InvalidateVisual();
    }

    private void PanViewportTo(Point mapPoint)
    {
        if (!TryGetMapGeometry(out var worldBounds, out var mapBounds, out var scale))
        {
            return;
        }

        var editor = Editor;
        if (editor == null || scale <= 0)
        {
            return;
        }

        var clampedX = Math.Clamp(mapPoint.X, mapBounds.Left, mapBounds.Right);
        var clampedY = Math.Clamp(mapPoint.Y, mapBounds.Top, mapBounds.Bottom);
        var worldX = worldBounds.Left + (clampedX - mapBounds.Left) / scale;
        var worldY = worldBounds.Top + (clampedY - mapBounds.Top) / scale;

        editor.OffsetX = -worldX + editor.Bounds.Width / 2d;
        editor.OffsetY = -worldY + editor.Bounds.Height / 2d;
        editor.ViewTranslateTransform.X = editor.OffsetX;
        editor.ViewTranslateTransform.Y = editor.OffsetY;
        InvalidateVisual();
    }

    private bool TryGetMapGeometry(out Rect worldBounds, out Rect mapBounds, out double scale)
    {
        worldBounds = default;
        mapBounds = default;
        scale = 1d;

        var editor = Editor;
        if (editor == null || Bounds.Width <= 1 || Bounds.Height <= 1 || editor.Bounds.Width <= 1 || editor.Bounds.Height <= 1)
        {
            return false;
        }

        var viewportWorld = GetViewportWorldRect(editor);
        var graphWorld = GetNodesWorldRect(editor);
        worldBounds = graphWorld.HasValue ? Union(graphWorld.Value, viewportWorld) : viewportWorld;
        worldBounds = EnsureMinSize(Inflate(worldBounds, 20d), 120d, 80d);

        var availableWidth = Math.Max(1d, Bounds.Width - (MapPadding * 2d));
        var availableHeight = Math.Max(1d, Bounds.Height - (MapPadding * 2d));
        var scaleX = availableWidth / worldBounds.Width;
        var scaleY = availableHeight / worldBounds.Height;
        scale = Math.Min(scaleX, scaleY);
        if (scale <= 0 || double.IsNaN(scale))
        {
            return false;
        }

        var contentWidth = worldBounds.Width * scale;
        var contentHeight = worldBounds.Height * scale;
        var offsetX = (Bounds.Width - contentWidth) / 2d;
        var offsetY = (Bounds.Height - contentHeight) / 2d;
        mapBounds = new Rect(offsetX, offsetY, contentWidth, contentHeight);
        return true;
    }

    private static Rect GetViewportWorldRect(NodifyEditor editor)
    {
        return new Rect(-editor.OffsetX, -editor.OffsetY, Math.Max(1d, editor.Bounds.Width), Math.Max(1d, editor.Bounds.Height));
    }

    private static Rect? GetNodesWorldRect(NodifyEditor editor)
    {
        Rect? result = null;
        foreach (var node in GetVisibleNodes(editor))
        {
            var nodeRect = new Rect(node.Location, GetNodeSize(node));
            result = result == null ? nodeRect : Union(result.Value, nodeRect);
        }

        return result;
    }

    private static IEnumerable<BaseNode> GetVisibleNodes(NodifyEditor editor)
    {
        var host = editor.GetChildOfType<Canvas>("NodeItemsPresenter");
        if (host == null)
        {
            yield break;
        }

        foreach (var visual in host.GetVisualChildren())
        {
            if (visual is not Control container)
            {
                continue;
            }

            var node = container.GetVisualChildren().FirstOrDefault() as BaseNode;
            if (node != null)
            {
                yield return node;
            }
        }
    }

    private static Size GetNodeSize(BaseNode node)
    {
        var boundsSize = node.Bounds.Size;
        var desiredSize = node.DesiredSize;
        var width = Math.Max(6d, boundsSize.Width > 0 ? boundsSize.Width : desiredSize.Width);
        var height = Math.Max(4d, boundsSize.Height > 0 ? boundsSize.Height : desiredSize.Height);
        return new Size(width, height);
    }

    private static Rect WorldToMap(Rect worldRect, Rect worldBounds, Rect mapBounds, double scale)
    {
        var x = mapBounds.X + (worldRect.X - worldBounds.X) * scale;
        var y = mapBounds.Y + (worldRect.Y - worldBounds.Y) * scale;
        var width = Math.Max(1d, worldRect.Width * scale);
        var height = Math.Max(1d, worldRect.Height * scale);
        return new Rect(x, y, width, height);
    }

    private static Rect Inflate(Rect rect, double amount)
    {
        return new Rect(rect.Left - amount, rect.Top - amount, rect.Width + amount * 2d, rect.Height + amount * 2d);
    }

    private static Rect EnsureMinSize(Rect rect, double minWidth, double minHeight)
    {
        var width = Math.Max(minWidth, rect.Width);
        var height = Math.Max(minHeight, rect.Height);
        var centerX = rect.X + rect.Width / 2d;
        var centerY = rect.Y + rect.Height / 2d;
        return new Rect(centerX - width / 2d, centerY - height / 2d, width, height);
    }

    private static Rect Union(Rect a, Rect b)
    {
        var left = Math.Min(a.Left, b.Left);
        var top = Math.Min(a.Top, b.Top);
        var right = Math.Max(a.Right, b.Right);
        var bottom = Math.Max(a.Bottom, b.Bottom);
        return new Rect(left, top, right - left, bottom - top);
    }
}
