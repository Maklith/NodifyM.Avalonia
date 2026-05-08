using Avalonia;
using Avalonia.Interactivity;
using NodifyM.Avalonia.Controls;

namespace NodifyM.Avalonia.Events
{
    /// <summary>
    /// Represents the method that will handle <see cref="BaseConnection"/> related routed events.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void ConnectionEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// Provides data for <see cref="BaseConnection"/> related routed events.
    /// </summary>
    public class ConnectionEventArgs : RoutedEventArgs
    {
       
        public ConnectionEventArgs(object connection)
            => Connection = connection;

        /// <summary>
        /// Gets or sets the location where the connection should be split.
        /// </summary>
        public Point SplitLocation { get; set; }

        public object Connection { get; }

       
    }
}
