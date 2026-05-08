using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace NodifyM.Avalonia.Controls;

public class NodeInput : Connector
{
    
    public static readonly AvaloniaProperty<object> HeaderProperty = AvaloniaProperty.Register<NodeInput, object>(nameof(Header));
    public static readonly AvaloniaProperty<IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<NodeInput, IDataTemplate>(nameof(HeaderTemplate));
    public static readonly AvaloniaProperty<ControlTemplate> ConnectorTemplateProperty = AvaloniaProperty.Register<NodeInput, ControlTemplate>(nameof(ConnectorTemplate));
    
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the content of the control's header.
    /// </summary>
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>
    /// Gets or sets the template used to display the connecting point of this <see cref="Connector"/>.
    /// </summary>
    public ControlTemplate? ConnectorTemplate
    {
        get => (ControlTemplate?)GetValue(ConnectorTemplateProperty);
        set => SetValue(ConnectorTemplateProperty, value);
    }
}