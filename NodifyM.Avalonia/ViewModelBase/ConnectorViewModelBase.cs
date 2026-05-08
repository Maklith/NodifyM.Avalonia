using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class ConnectorViewModelBase : ObservableObject
{
    public enum ConnectorFlow
    {
        Input,
        Output
    }
    [ObservableProperty]
    private Point _anchor;
    
    [ObservableProperty]
    private string _title="";
    [ObservableProperty]
    private bool _isConnected;
    [ObservableProperty]
    private bool _canConnect;
    public ConnectorFlow? Flow { get;  set; }
}