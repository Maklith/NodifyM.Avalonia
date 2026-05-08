using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using NodifyM.Avalonia.Events;
using NodifyM.Avalonia.Helpers;

namespace NodifyM.Avalonia.Controls;

public class PendingConnection : ContentControl
{
    #region Dependency Properties
    
        public static readonly StyledProperty<Point> SourceAnchorProperty=AvaloniaProperty.Register<PendingConnection, Point>(nameof(SourceAnchor), BoxValue.Point);
        public static readonly StyledProperty<Point> TargetAnchorProperty = AvaloniaProperty.Register<PendingConnection, Point>(nameof(TargetAnchor), BoxValue.Point);
        public static readonly AvaloniaProperty SourceProperty = AvaloniaProperty.Register<PendingConnection, object>(nameof(Source));
        public static readonly AvaloniaProperty TargetProperty = AvaloniaProperty.Register<PendingConnection, object>(nameof(Target));
        public static readonly AvaloniaProperty PreviewTargetProperty = AvaloniaProperty.Register<PendingConnection, object>(nameof(PreviewTarget));
        public static readonly AvaloniaProperty EnablePreviewProperty = AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnablePreview),BoxValue.True);
        public static readonly AvaloniaProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty.AddOwner<PendingConnection>();
        public static readonly AvaloniaProperty StrokeDashArrayProperty = Shape.StrokeDashArrayProperty.AddOwner<PendingConnection>();
        public static readonly AvaloniaProperty StrokeProperty = Shape.StrokeProperty.AddOwner<PendingConnection>();
        public static readonly AvaloniaProperty AllowOnlyConnectorsProperty = AvaloniaProperty.Register<PendingConnection, bool>(nameof(AllowOnlyConnectors),BoxValue.True);
        public static readonly AvaloniaProperty EnableSnappingProperty = AvaloniaProperty.Register<PendingConnection, bool>(nameof(EnableSnapping),BoxValue.False);
        public static readonly AvaloniaProperty DirectionProperty = BaseConnection.DirectionProperty.AddOwner<PendingConnection>();
        
        /// <summary>
        /// Gets or sets the starting point for the connection.
        /// </summary>
        public Point SourceAnchor
        {
            get => GetValue(SourceAnchorProperty);
            set => SetValue(SourceAnchorProperty, value);
        }

        /// <summary>
        /// Gets or sets the end point for the connection.
        /// </summary>
        public Point TargetAnchor
        {
            get => GetValue(TargetAnchorProperty);
            set => SetValue(TargetAnchorProperty, value);
        }
        
        public object? Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        
        public object? Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }
        
        public bool EnablePreview
        {
            get => (bool)GetValue(EnablePreviewProperty)!;
            set => SetValue(EnablePreviewProperty, value);
        }

    
        public object? PreviewTarget
        {
            get => GetValue(PreviewTargetProperty);
            set => SetValue(PreviewTargetProperty, value);
        }

    
        public bool EnableSnapping
        {
            get => (bool)GetValue(EnableSnappingProperty)!;
            set => SetValue(EnableSnappingProperty, value);
        }
        
        public bool AllowOnlyConnectors
        {
            get => (bool)GetValue(AllowOnlyConnectorsProperty)!;
            set => SetValue(AllowOnlyConnectorsProperty, value);
        }

        /// <summary>
        /// Gets or set the connection thickness.
        /// </summary>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty)!;
            set => SetValue(StrokeThicknessProperty, value);
        }

        public AvaloniaList<double>? StrokeDashArray
        {
            get => (AvaloniaList<double>?)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }
        

        /// <summary>
        /// Gets or sets the stroke color of the connection.
        /// </summary>
        public IBrush? Stroke
        {
            get => (Brush?)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        

        /// <summary>
        /// Gets or sets the direction of this connection.
        /// </summary>
        public ConnectionDirection Direction
        {
            get => (ConnectionDirection)GetValue(DirectionProperty)!;
            set => SetValue(DirectionProperty, value);
        }

        #endregion
        
        public static readonly AvaloniaProperty StartedCommandProperty = AvaloniaProperty.Register<PendingConnection,ICommand>(nameof(StartedCommand));
        public static readonly AvaloniaProperty CompletedCommandProperty = AvaloniaProperty.Register<PendingConnection,ICommand>(nameof(CompletedCommand));
        
        public ICommand? StartedCommand
        {
            get => (ICommand?)GetValue(StartedCommandProperty);
            set => SetValue(StartedCommandProperty, value);
        }
        
        public ICommand? CompletedCommand
        {
            get => (ICommand?)GetValue(CompletedCommandProperty);
            set => SetValue(CompletedCommandProperty, value);
        }
        private static readonly AvaloniaProperty AllowOnlyConnectorsAttachedProperty = AvaloniaProperty.RegisterAttached<PendingConnection,bool>("AllowOnlyConnectorsAttached",typeof(PendingConnection),BoxValue.True);
        
        public static readonly AvaloniaProperty IsOverElementProperty = AvaloniaProperty.RegisterAttached<PendingConnection,bool>("IsOverElement", typeof(PendingConnection), (BoxValue.False));

        internal static bool GetAllowOnlyConnectorsAttached(Control elem)
            => (bool)(elem.GetValue(AllowOnlyConnectorsAttachedProperty) ?? throw new InvalidOperationException());

        private NodifyEditor? Editor { get; set; }
        private Control? _previousConnector;
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Editor = this.GetParentOfType<NodifyEditor>();
            IsVisible = false;
            if (Editor != null)
            {
                Editor.AddHandler(Connector.PendingConnectionStartedEvent,new EventHandler<PendingConnectionEventArgs>(OnPendingConnectionStarted));
                Editor.AddHandler(Connector.PendingConnectionDragEvent,new EventHandler<PendingConnectionEventArgs>(OnPendingConnectionDrag));
                Editor.AddHandler(Connector.PendingConnectionCompletedEvent,new EventHandler<PendingConnectionEventArgs>(OnPendingConnectionCompleted));
            }
        }
        protected virtual void OnPendingConnectionStarted(object? sender, PendingConnectionEventArgs e)
        {
            if (!e.Handled && !e.Canceled)
            {
                e.Handled = true;
                e.Canceled = !StartedCommand?.CanExecute(e.SourceConnector) ?? false;
                    
                Target = null;
                IsVisible = !e.Canceled;
                SourceAnchor = e.Anchor;
                TargetAnchor = new Point(e.Anchor.X + e.OffsetX, e.Anchor.Y + e.OffsetY);
                Source = e.SourceConnector;

                if (!e.Canceled)
                {
                    StartedCommand?.Execute(Source);
                }

                if(EnablePreview)
                {
                    PreviewTarget = e.SourceConnector;
                }
            }
        }

        protected virtual void OnPendingConnectionDrag(object? sender, PendingConnectionEventArgs e)
        {
            if (!e.Handled && IsVisible)
            {
                e.Handled = true;
                TargetAnchor = new Point(e.Anchor.X + e.OffsetX, e.Anchor.Y + e.OffsetY);

                if (Editor != null && (EnablePreview || EnableSnapping))
                {
                    // Look for a potential connector
                    Control? connector = GetPotentialConnector(Editor, AllowOnlyConnectors,TargetAnchor);
                    // Update the connector's anchor and snap to it if snapping is enabled
                    if (EnableSnapping && connector is Connector target)
                    {
                        TargetAnchor = target.Anchor;
                    }

                    // If it's not the same connector
                    if (connector != _previousConnector)
                    {
                        if (_previousConnector != null)
                        {
                            //SetIsOverElement(_previousConnector, false);
                        }
                    
                        // And we have a connector
                        if (connector != null)
                        {
                            //SetIsOverElement(connector, true);
                        }
                    
                        // Update the preview target if enabled
                        if (EnablePreview)
                        {
                            PreviewTarget = connector?.DataContext;
                           // Target = connector?.DataContext;
                        }
                    
                        _previousConnector = connector;
                    }
                }
            }
        }

        protected virtual void OnPendingConnectionCompleted(object? sender, PendingConnectionEventArgs e)
        {
            if (!e.Handled && IsVisible)
            {
                e.Handled = true;
                IsVisible = false;

                var previousConnector = _previousConnector;
                if (previousConnector != null)
                {
                    //. SetIsOverElement(_previousConnector, false);
                    _previousConnector = null;
                }

                if (!e.Canceled)
                {
                    Target = e.TargetConnector;
                    // Invoke the CompletedCommand if event is not handled
                    if (CompletedCommand?.CanExecute(Target) ?? false)
                    {
                        CompletedCommand?.Execute(Target);
                    }
                }

                if(EnablePreview)
                {
                    PreviewTarget = null;
                }
            }
        }
        internal static Control? GetPotentialConnector(NodifyEditor editor, bool allowOnlyConnectors,Point anchor)
        {
            Connector? connector = editor.ItemsPanelRoot!.GetVisualAt<Connector>(anchor);
            
            if (connector != null && connector.Editor == editor)
            {
                connector.UpdateAnchor();
                return connector; 
            }

            if (allowOnlyConnectors)
                return null;
            

            return editor;
        }
}