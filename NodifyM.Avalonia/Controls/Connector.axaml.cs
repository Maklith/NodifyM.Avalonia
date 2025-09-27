using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;

namespace NodifyM.Avalonia.Controls;

public class Connector : ContentControl
{
    protected const string ElementConnector = "PART_Connector";

    /// <summary>
    /// Gets the <see cref="NodifyEditor"/> that owns this <see cref="Container"/>.
    /// </summary>
    private Point _lastUpdatedContainerPosition;

    private Point _thumbCenter;

    static Connector()
    {
        //DefaultStyleKeyProperty.OverrideMetadata(typeof(Connector), new FrameworkPropertyMetadata(typeof(Connector)));
        FocusableProperty.OverrideMetadata(typeof(Connector), new StyledPropertyMetadata<bool>(BoxValue.True));
    }

    protected internal NodifyEditor? Editor { get; private set; }
    protected Control? Thumb { get; private set; }
    protected BaseNode? Container { get; private set; }
    public static bool AllowPendingConnectionCancellation { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the connection should be completed in two steps.
    /// </summary>
    public static bool EnableStickyConnections { get; set; }

    private void OnConnectorLoaded()
        => TrySetAnchorUpdateEvents(true);

    private void OnConnectorUnloaded()
        => TrySetAnchorUpdateEvents(false);

    private void TrySetAnchorUpdateEvents(bool value)
    {
        if (Container != null && Editor != null)
        {
            // If events are not already hooked and we are asked to subscribe
            if (value)
            {
                Container.LocationChanged += OnLocationChanged;
                Container.SizeChanged += OnContainerSizeChanged;
            }
            // If events are already hooked and we are asked to unsubscribe
            else if (!value)
            {
                Container.LocationChanged -= OnLocationChanged;
                Container.SizeChanged -= OnContainerSizeChanged;
            }
        }
    }

    private void OnContainerSizeChanged(object sender, SizeChangedEventArgs e)
        => UpdateAnchor(Container!.Location);

    private void OnLocationChanged(object sender, RoutedEventArgs e)
        => UpdateAnchor(Container!.Location);

    public void UpdateAnchor()
    {
        if (Container != null)
        {
            UpdateAnchor(Container.Location);
        }
    }

    /// <summary>
    /// Updates the <see cref="Anchor"/> and applies optimizations if needed based on <see cref="EnableOptimizations"/> flag
    /// </summary>
    /// <param name="location"></param>
    protected void UpdateAnchor(Point location)
    {
        _lastUpdatedContainerPosition = location;
        if (Container != null)
        {
            Size containerMargin = (Size)Container.Bounds.Size - (Size)Container.DesiredSize;
            Point relativeLocation = Thumb.TranslatePoint(new Point((Thumb.Bounds.Width - containerMargin.Width) / 2,
                (Thumb.Bounds.Height - containerMargin.Height) / 2), Container)!.Value;
            Anchor = new Point(location.X + relativeLocation.X, location.Y + relativeLocation.Y);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Container = this.GetParentOfType<BaseNode>();

        Editor = this.GetParentOfType<NodifyEditor>();
        Thumb = this.GetChildOfType<Control>("PART_Connector");
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (Editor is null)
        {
            return;
        }


        var currentPoint = e.GetCurrentPoint(this);
        if (currentPoint.Properties.IsLeftButtonPressed && e.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            e.GetCurrentPoint(this)
                .Pointer.Capture(this);
            e.Handled = true;
            OnDisconnect();
        }
        else if (currentPoint.Properties.IsLeftButtonPressed)
        {
            if (EnableStickyConnections && IsPendingConnection)
            {
                e.GetCurrentPoint(this)
                    .Pointer.Capture(this);
                e.Handled = true;
                OnConnectorDragCompleted(e.GetPosition(Editor));
            }
            else
            {
                e.GetCurrentPoint(this)
                    .Pointer.Capture(this);
                e.Handled = true;
                Editor.SelectItem(this.GetParentOfType<BaseNode>(), e.KeyModifiers.HasFlag(KeyModifiers.Control));

                UpdateAnchor();
                OnConnectorDragStarted();
            }
        }
    }


    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (Editor is null)
        {
            return;
        }

        if (IsPendingConnection)
        {
            Vector offset = e.GetPosition(Thumb) - _thumbCenter;
            OnConnectorDrag(offset);
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (Editor is null)
        {
            return;
        }

        Vector offset = e.GetPosition(Thumb) - _thumbCenter;
        var point = new Point(Anchor.X + offset.X, Anchor.Y + offset.Y);
        var currentPoint = e.GetCurrentPoint(this);
        e.Handled = EnableStickyConnections && IsPendingConnection;
        if (!EnableStickyConnections && e.InitialPressMouseButton.HasFlag(MouseButton.Left))
        {
            OnConnectorDragCompleted(point);
            e.Handled = true;
        }
        else if (AllowPendingConnectionCancellation && IsPendingConnection &&
                 (currentPoint.Properties.IsRightButtonPressed))
        {
            // Cancel pending connection
            OnConnectorDragCompleted(point, true);
            // ReleaseMouseCapture();

            // Don't show context menu
            e.Handled = true;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (Editor is null)
        {
            return;
        }

        if (AllowPendingConnectionCancellation && e.Key.HasFlag(Key.Escape))
        {
            // Cancel pending connection
            OnConnectorDragCompleted(cancel: true);
        }
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        if (Editor is null)
        {
            return;
        }

        OnConnectorDragCompleted(cancel: true);
    }

    protected virtual void OnDisconnect()
    {
        if (IsConnected && !IsPendingConnection)
        {
            object? connector = DataContext;
            var args = new ConnectorEventArgs(connector)
            {
                RoutedEvent = DisconnectEvent,
                Anchor = Anchor,
                Source = this
            };

            RaiseEvent(args);

            // Raise DisconnectCommand if event is Disconnect not handled
            if (!args.Handled && (DisconnectCommand?.CanExecute(connector) ?? false))
            {
                DisconnectCommand.Execute(connector);
            }
        }
    }

    protected virtual void OnConnectorDrag(Vector offset)
    {
        var args = new PendingConnectionEventArgs(DataContext)
        {
            RoutedEvent = PendingConnectionDragEvent,
            OffsetX = offset.X,
            OffsetY = offset.Y,
            Anchor = Anchor,
            Source = this
        };

        RaiseEvent(args);
    }

    protected virtual void OnConnectorDragStarted()
    {
        if (Thumb != null)
        {
            _thumbCenter = new Point(Thumb.Bounds.Width / 2, Thumb.Bounds.Height / 2);
        }

        var args = new PendingConnectionEventArgs(DataContext)
        {
            RoutedEvent = PendingConnectionStartedEvent,
            Anchor = Anchor,
            Source = this
        };

        RaiseEvent(args);
        IsPendingConnection = !args.Canceled;
    }

    protected virtual void OnConnectorDragCompleted(Point? point = null, bool cancel = false)
    {
        if (IsPendingConnection)
        {
            Control? elem = (Editor != null && point.HasValue)
                ? PendingConnection.GetPotentialConnector(Editor,
                    PendingConnection.GetAllowOnlyConnectorsAttached(Editor), point.Value)
                : null;

            var args = new PendingConnectionEventArgs(DataContext)
            {
                TargetConnector = elem?.DataContext,
                RoutedEvent = PendingConnectionCompletedEvent,
                Anchor = Anchor,
                Source = this,
                Canceled = cancel
            };

            IsPendingConnection = false;
            RaiseEvent(args);
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        UpdateAnchor();
        OnConnectorLoaded();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        OnConnectorUnloaded();
    }

    #region Routed Events

    public static readonly RoutedEvent PendingConnectionStartedEvent =
        RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionStarted),
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent PendingConnectionCompletedEvent =
        RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionCompleted),
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent PendingConnectionDragEvent =
        RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(PendingConnectionDrag),
            RoutingStrategies.Bubble);

    public static readonly RoutedEvent DisconnectEvent =
        RoutedEvent.Register<Connector, PendingConnectionEventArgs>(nameof(Disconnect), RoutingStrategies.Bubble);


    /// <summary>Triggered by the <see cref="Connector.Connect"/> gesture.</summary>
    public event PendingConnectionEventHandler PendingConnectionStarted
    {
        add => AddHandler(PendingConnectionStartedEvent, value);
        remove => RemoveHandler(PendingConnectionStartedEvent, value);
    }

    /// <summary>Triggered by the <see cref="Connector.Connect"/> gesture.</summary>
    public event PendingConnectionEventHandler PendingConnectionCompleted
    {
        add => AddHandler(PendingConnectionCompletedEvent, value);
        remove => RemoveHandler(PendingConnectionCompletedEvent, value);
    }

    /// <summary>
    /// Occurs when the mouse is changing position and the <see cref="Connector"/> has mouse capture.
    /// </summary>
    public event PendingConnectionEventHandler PendingConnectionDrag
    {
        add => AddHandler(PendingConnectionDragEvent, value);
        remove => RemoveHandler(PendingConnectionDragEvent, value);
    }

    /// <summary>Triggered by the <see cref="Connector.Disconnect"/> gesture.</summary>
    public event ConnectorEventHandler Disconnect
    {
        add => AddHandler(DisconnectEvent, value);
        remove => RemoveHandler(DisconnectEvent, value);
    }

    #endregion

    #region Dependency Properties

    public static readonly StyledProperty<Point> AnchorProperty =
        AvaloniaProperty.Register<Connector, Point>(nameof(Anchor), BoxValue.Point);

    public static readonly StyledProperty<bool> IsConnectedProperty =
        AvaloniaProperty.Register<Connector, bool>(nameof(Anchor), BoxValue.False);

    public static readonly AvaloniaProperty<ICommand> DisconnectCommandProperty =
        AvaloniaProperty.Register<Connector, ICommand>(nameof(DisconnectCommand));

    public static readonly AvaloniaProperty<bool> IsPendingConnectionProperty =
        AvaloniaProperty.Register<Connector, bool>(nameof(IsPendingConnection), BoxValue.False);


    /// <summary>
    /// Gets the location where <see cref="Connection"/>s can be attached to. 
    /// Bind with <see cref="BindingMode.OneWayToSource"/>
    /// </summary>
    public Point Anchor
    {
        get => (Point)GetValue(AnchorProperty);
        set => SetValue(AnchorProperty, value);
    }

    /// <summary>
    /// If this is set to false, the <see cref="Disconnect"/> event will not be invoked and the connector will stop updating its <see cref="Anchor"/> when moved, resized etc.
    /// </summary>
    public bool IsConnected
    {
        get => (bool)GetValue(IsConnectedProperty);
        set => SetValue(IsConnectedProperty, value);
    }

    /// <summary>
    /// Gets a value that indicates whether a <see cref="PendingConnection"/> is in progress for this <see cref="Connector"/>.
    /// </summary>
    public bool IsPendingConnection
    {
        get => (bool)GetValue(IsPendingConnectionProperty);
        protected set => SetValue(IsPendingConnectionProperty, value);
    }

    /// <summary>
    /// Invoked if the <see cref="Disconnect"/> event is not handled.
    /// Parameter is the <see cref="FrameworkElement.DataContext"/> of this control.
    /// </summary>
    public ICommand? DisconnectCommand
    {
        get => (ICommand?)GetValue(DisconnectCommandProperty);
        set => SetValue(DisconnectCommandProperty, value);
    }

    #endregion
}