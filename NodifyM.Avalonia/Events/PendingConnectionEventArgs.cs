using Avalonia;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events
{
    /// <summary>
    /// Represents the method that will handle <see cref="PendingConnection"/> related routed events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void PendingConnectionEventHandler(object sender, PendingConnectionEventArgs e);
    
    /// <summary>
    /// Provides data for <see cref="PendingConnection"/> related routed events.
    /// </summary>
    public class PendingConnectionEventArgs : RoutedEventArgs
    {
        
        public PendingConnectionEventArgs(object sourceConnector)
            => SourceConnector = sourceConnector;
        
        /// <summary>
        /// Gets or sets the <see cref="Connector.Anchor"/> of the <see cref="Connector"/> that raised this event.
        /// </summary>
        public Point Anchor { get; set; }
        
       
        public object SourceConnector { get; }
        
        public object? TargetConnector { get; set; }

        /// <summary>
        /// Gets or sets the distance from the <see cref="SourceConnector"/> in the X axis.
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the distance from the <see cref="SourceConnector"/> in the Y axis.
        /// </summary>
        public double OffsetY { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether this <see cref="PendingConnection"/> was cancelled.
        /// </summary>
        public bool Canceled { get; set; }

        
    }
}
