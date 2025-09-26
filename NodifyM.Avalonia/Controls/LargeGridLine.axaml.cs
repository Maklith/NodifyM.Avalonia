using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace NodifyM.Avalonia.Controls;

public class LargeGridLine : TemplatedControl
{
    public static readonly AvaloniaProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<LargeGridLine, double>(nameof(OffsetX));

    public static readonly AvaloniaProperty<double> OffsetYProperty =
        AvaloniaProperty.Register<LargeGridLine, double>(nameof(OffsetY));

    public static readonly AvaloniaProperty<double> ZoomProperty =
        AvaloniaProperty.Register<LargeGridLine, double>(nameof(Zoom));

    public static readonly AvaloniaProperty<IBrush> BrushProperty =
        AvaloniaProperty.Register<LargeGridLine, IBrush>(nameof(Brush), Brushes.Gainsboro);

    public static readonly AvaloniaProperty<double> ThicknessProperty =
        AvaloniaProperty.Register<LargeGridLine, double>(nameof(Thickness), 0.5);

    public static readonly AvaloniaProperty<double> SpacingProperty =
        AvaloniaProperty.Register<LargeGridLine, double>(nameof(Spacing), 20);

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public IBrush Brush
    {
        get { return (IBrush)GetValue(BrushProperty); }
        set { SetValue(BrushProperty, value); }
    }

    public double Thickness
    {
        get { return (double)GetValue(ThicknessProperty); }
        set { SetValue(ThicknessProperty, value); }
    }

    public double OffsetX
    {
        get { return (double)GetValue(OffsetXProperty); }
        set { SetValue(OffsetXProperty, value); }
    }

    public double OffsetY
    {
        get { return (double)GetValue(OffsetYProperty); }
        set { SetValue(OffsetYProperty, value); }
    }

    public double Zoom
    {
        get { return (double)GetValue(ZoomProperty); }
        set { SetValue(ZoomProperty, value); }
    }

    public List<IDisposable> Disposables { get; } = new List<IDisposable>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Disposables.Add(OffsetXProperty.Changed.AddClassHandler<LargeGridLine>(ToInvalidateVisual));
        Disposables.Add(OffsetYProperty.Changed.AddClassHandler<LargeGridLine>(ToInvalidateVisual));
    }

    private void ToInvalidateVisual(LargeGridLine largeGridLine,
        AvaloniaPropertyChangedEventArgs avaloniaPropertyChangedEventArgs)
    {
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var pen = new Pen(Brush, Thickness);
        double step = Spacing;
        // Draw horizontal lines
        var offsetY = Math.Abs(OffsetY / Zoom);
        var offsetX = Math.Abs(OffsetX / Zoom);
        for (double y = OffsetY % step; y < this.Bounds.Height; y += step)
        {
            context.DrawLine(pen, new Point(-offsetX, y), new Point(this.Bounds.Width, y));
        }

        // Draw vertical lines

        for (double x = OffsetX % step; x < this.Bounds.Width; x += step)
        {
            context.DrawLine(pen, new Point(x, -offsetY), new Point(x, this.Bounds.Height));
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        foreach (var disposable in Disposables)
        {
            disposable.Dispose();
        }
    }
}