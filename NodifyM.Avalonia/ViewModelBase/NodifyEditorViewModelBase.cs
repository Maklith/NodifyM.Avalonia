using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class NodifyEditorViewModelBase : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<object?> _nodes = new();

    [ObservableProperty]
    private ObservableCollection<object?> _selectedNodes = new();

    [ObservableProperty]
    private ObservableCollection<ConnectionViewModelBase> _connections=new ();

    public PendingConnectionViewModelBase PendingConnection { get; set; }

    public NodifyEditorViewModelBase()
    {
        PendingConnection = new PendingConnectionViewModelBase(this);
    }
    [RelayCommand]
    public virtual void DisconnectConnector(ConnectorViewModelBase connector)
    {
        var connections = Connections.Where(e => e.Source == connector || e.Target == connector).ToList();
        for (var i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            Connections.Remove(connection);
            if (Connections.All(e => e.Source != connection.Source))
            {
                connection.Source.IsConnected = false;
            }

            if (Connections.All(e => e.Target != connection.Target))
            {
                connection.Target.IsConnected = false;
            }

            
        }
        
    }
    
    public virtual void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        switch (source.Flow)
        {
            case ConnectorViewModelBase.ConnectorFlow.Output:
                Connections.Add(new ConnectionViewModelBase(this,source, target));
                break;
            case ConnectorViewModelBase.ConnectorFlow.Input:
                Connections.Add(new ConnectionViewModelBase(this,target, source));
                break;
            default:
            {
                switch (target.Flow)
                {
                    case ConnectorViewModelBase.ConnectorFlow.Input:
                        Connections.Add(new ConnectionViewModelBase(this,source, target));
                        break;
                    case ConnectorViewModelBase.ConnectorFlow.Output:
                        Connections.Add(new ConnectionViewModelBase(this,target, source));
                        break;
                    default:
                        Connections.Add(new ConnectionViewModelBase(this,source, target));
                        break;
                }

                break;
            }
        }
        
        source.IsConnected = true;
        target.IsConnected = true;
    }

    
}