using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.VisualTree;
using NodifyM.Avalonia.ViewModelBase;
using System.Collections;
using System.Diagnostics;

namespace NodifyM.Avalonia.Controls;

public class Node : BaseNode
{
    public static readonly AvaloniaProperty<IBrush> ContentBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(ContentBrush));

    public static readonly AvaloniaProperty<IBrush> HeaderBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(HeaderBrush));

    public static readonly AvaloniaProperty<IBrush> FooterBrushProperty =
        AvaloniaProperty.Register<Node, IBrush>(nameof(FooterBrush));

    public static readonly AvaloniaProperty<object> FooterProperty =
        AvaloniaProperty.Register<Node, object>(nameof(Footer));

    public static readonly AvaloniaProperty<object> HeaderProperty =
        AvaloniaProperty.Register<Node, object>(nameof(Header));

    public static readonly AvaloniaProperty<IDataTemplate> FooterTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(FooterTemplate));

    public static readonly AvaloniaProperty<IDataTemplate> HeaderTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(HeaderTemplate));

    public static readonly AvaloniaProperty<IDataTemplate> InputConnectorTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(InputConnectorTemplate));

    protected internal static readonly AvaloniaProperty<bool> HasFooterProperty =
        AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasFooter), o => o.HasFooter);

    protected internal static readonly AvaloniaProperty<bool> HasHeaderProperty =
        AvaloniaProperty.RegisterDirect<Node, bool>(nameof(HasHeader), o => o.HasHeader);

    public static readonly StyledProperty<IDataTemplate> OutputConnectorTemplateProperty =
        AvaloniaProperty.Register<Node, IDataTemplate>(nameof(OutputConnectorTemplate));

    public static readonly AvaloniaProperty<IEnumerable> InputProperty =
        AvaloniaProperty.Register<Node, IEnumerable>(nameof(Input));

    public static readonly AvaloniaProperty<IEnumerable> OutputProperty =
        AvaloniaProperty.Register<Node, IEnumerable>(nameof(Output));
    public static readonly AvaloniaProperty<bool> IsResizeProperty =
      AvaloniaProperty.Register<Node, bool>(nameof(IsResize),false);


    public Brush ContentBrush
    {
        get => (Brush)GetValue(ContentBrushProperty);
        set => SetValue(ContentBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for the background of the <see cref="HeaderedContentControl.Header"/> of this <see cref="Node"/>.
    /// </summary>
    public Brush HeaderBrush
    {
        get => (Brush)GetValue(HeaderBrushProperty);
        set => SetValue(HeaderBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the brush used for the background of the <see cref="Node.Footer"/> of this <see cref="Node"/>.
    /// </summary>
    public Brush FooterBrush
    {
        get => (Brush)GetValue(FooterBrushProperty);
        set => SetValue(FooterBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the data for the footer of this control.
    /// </summary>
    public object Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's footer.
    /// </summary>
    public IDataTemplate FooterTemplate
    {
        get => (DataTemplate)GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    public IDataTemplate HeaderTemplate
    {
        get => (DataTemplate)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Input"/> connectors.
    /// </summary>
    public IDataTemplate InputConnectorTemplate
    {
        get => (DataTemplate)GetValue(InputConnectorTemplateProperty);
        set => SetValue(InputConnectorTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's <see cref="Output"/> connectors.
    /// </summary>
    public IDataTemplate OutputConnectorTemplate
    {
        get => (DataTemplate)GetValue(OutputConnectorTemplateProperty);
        set => SetValue(OutputConnectorTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the data for the input <see cref="Connector"/>s of this control.
    /// </summary>
    public IEnumerable Input
    {
        get => (IEnumerable)GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    /// <summary>
    /// Gets or sets the data for the output <see cref="Connector"/>s of this control.
    /// </summary>
    public IEnumerable Output
    {
        get => (IEnumerable)GetValue(OutputProperty);
        set => SetValue(OutputProperty, value);
    }
    /// <summary>
    /// Get or sets a value indicating whether the Node can be resized.
    /// </summary>
    public bool IsResize
    {
        get => (bool)GetValue(IsResizeProperty);
        set => SetValue(IsResizeProperty, value);
    }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="Footer"/> is <see langword="null" />.
    /// </summary>
    public bool HasFooter => GetValue(FooterProperty) != null;

    public bool HasHeader => GetValue(HeaderProperty) != null;

    public Node()
    {
    }
    private Thumb _topLeft, _topRight, _bottomLeft, _bottomRight;
    private Vector _startPoint;
    private Size _startSize;
    private Point _startControlPosition;
    private bool _isDragging;
    private TranslateTransform _dragTransform;
    private Vector _totalDelta; // 记录累计位移
    Vector _lastPoint;
 

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // 查找拖拽手柄
        _topLeft = e.NameScope.Find<Thumb>("PART_TopLeft");
        _topRight = e.NameScope.Find<Thumb>("PART_TopRight");
        _bottomLeft = e.NameScope.Find<Thumb>("PART_BottomLeft");
        _bottomRight = e.NameScope.Find<Thumb>("PART_BottomRight");

        // 绑定拖拽事件
        BindThumbEvents(_topLeft, HandleTopLeftDrag);
        BindThumbEvents(_topRight, HandleTopRightDrag);
        BindThumbEvents(_bottomLeft, HandleBottomLeftDrag);
        BindThumbEvents(_bottomRight, HandleBottomRightDrag);

        // 初始化渲染变换
        _dragTransform = new TranslateTransform();
        RenderTransform = _dragTransform;

    }

    private void BindThumbEvents(Thumb thumb, Action<VectorEventArgs> dragHandler)
    {
        if (thumb == null) return;

        thumb.DragStarted += (s, e) => OnDragStarted(s, e);
        thumb.DragDelta += (s, e) => dragHandler(e);
        thumb.DragCompleted += (s, e) => OnDragCompleted(s, e);


    }
    private void OnDragStarted(object sender, VectorEventArgs e)
    {
        _isDragging = true;

        _startPoint = e.Vector;
        _lastPoint = new Vector();
        _startSize = new Size(this.Bounds.Width, this.Bounds.Height);
        _totalDelta = new Vector();
        // 获取控件在 Canvas 中的初始位置
        _startControlPosition = new Point(this.Location.X, this.Location.Y);
    }

    private void HandleTopLeftDrag(VectorEventArgs e)
    {
        var delta = e.Vector - _startPoint;
        delta = delta + _lastPoint;
        _lastPoint = delta;
        var newWidth = _startSize.Width - delta.X;
        var newHeight = _startSize.Height - delta.Y;
        ApplyResize(newWidth, newHeight, delta.X, delta.Y, true, true);
    }

    private void HandleTopRightDrag(VectorEventArgs e)
    {
        var delta = e.Vector - _startPoint;
        delta = delta + _lastPoint;
        _lastPoint = delta;
        var newWidth = _startSize.Width + delta.X;
        var newHeight = _startSize.Height - delta.Y;
        ApplyResize(newWidth, newHeight, 0, delta.Y, false, true);
    }

    private void HandleBottomLeftDrag(VectorEventArgs e)
    {
        var delta = e.Vector - _startPoint;
        delta = delta + _lastPoint;
        _lastPoint = delta;
        var newWidth = _startSize.Width - delta.X;
        var newHeight = _startSize.Height + delta.Y;
        ApplyResize(newWidth, newHeight, delta.X, 0, true, false);
    }


    private void HandleBottomRightDrag(VectorEventArgs e)
    {
        var delta = e.Vector - _startPoint;
        delta = delta + _lastPoint;
        _lastPoint = delta;

        var newWidth = _startSize.Width + delta.X;
        var newHeight = _startSize.Height + delta.Y;

        ApplyResize(newWidth, newHeight, 0, 0, false, false);
        e.Handled = true;
    }

    private void ApplyResize(double newWidth, double newHeight, double deltaX, double deltaY, bool moveX, bool moveY)
    {
        // 限制最小尺寸
        if (newWidth < MinWidth || newHeight < MinHeight)
            return;
        
        BeginInit();
        try
        {
            if (_isDragging)
            {
              
                if (moveX) _totalDelta = new Vector(deltaX, _totalDelta.Y);
                if (moveY) _totalDelta = new Vector(_totalDelta.X, deltaY);

                _dragTransform.X = _totalDelta.X;
                _dragTransform.Y = _totalDelta.Y;

               
                Width = newWidth;
                Height = newHeight;
            }
        }
        finally
        {
            EndInit();
        }

    }

    private void OnDragCompleted(object sender, VectorEventArgs e)
    {
        _isDragging = false;
        _dragTransform.X = 0;
        _dragTransform.Y = 0;
        this.Location = _startControlPosition + _totalDelta;

    }


}