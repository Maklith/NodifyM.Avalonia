using Avalonia;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events
{
    /// <summary>
    /// Represents the method that will handle <see cref="Connector"/> related routed events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void ConnectorEventHandler(object sender, ConnectorEventArgs e);

    /// <summary>
    /// Provides data for <see cref="Avalonia.Controls.Connector"/> related routed events.
    /// </summary>
    public class ConnectorEventArgs : RoutedEventArgs
    {
        
        public ConnectorEventArgs(object connector)
            => Connector = connector;

        /// <summary>
        /// Gets or sets the <see cref="Avalonia.Controls.Connector.Anchor"/> of the <see cref="Avalonia.Controls.Connector"/> associated with this event.
        /// </summary>
        public Point Anchor { get; set; }
        
        public object Connector { get; }

        
    }
}
