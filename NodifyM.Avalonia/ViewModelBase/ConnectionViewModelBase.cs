
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class ConnectionViewModelBase: ObservableObject
{
    [ObservableProperty] private ConnectorViewModelBase _source;
    [ObservableProperty] private ConnectorViewModelBase _target;
    [ObservableProperty] private string _text;

    private NodifyEditorViewModelBase NodifyEditor
    {
        get;
    }
    
    public ConnectionViewModelBase(NodifyEditorViewModelBase nodifyEditor,ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        this.NodifyEditor = nodifyEditor;
        Source = source;
        Target = target;
        Text = string.Empty;
    }
    public ConnectionViewModelBase(NodifyEditorViewModelBase nodifyEditor,ConnectorViewModelBase source, ConnectorViewModelBase target, string text)
    {
        this.NodifyEditor = nodifyEditor;
        Source = source;
        Target = target;
        Text = text;
    }
    [RelayCommand]
    public virtual void DisconnectConnection(ConnectionViewModelBase connection)
    {
        NodifyEditor.Connections.Remove(connection);
        if (NodifyEditor.Connections.All(e => (e.Source) != connection.Source&&(e.Target) != connection.Source))
        {
            connection.Source.IsConnected = false;
        }

        if (NodifyEditor.Connections.All(e => (e.Target != connection.Target)&&(e.Source != connection.Target)))
        {
            connection.Target.IsConnected = false;
        }
        
    }
    [RelayCommand]
    public virtual void SplitConnection(Point location)
    {
        var knot = new KnotNodeViewModel
        {
            Location = location,
            Connector = new ConnectorViewModelBase()
        };
        NodifyEditor.Nodes.Add(knot);

        NodifyEditor.Connect(Source, knot.Connector);
        NodifyEditor.Connect(knot.Connector, this.Target);

        NodifyEditor.Connections.Remove(this);
    }
}