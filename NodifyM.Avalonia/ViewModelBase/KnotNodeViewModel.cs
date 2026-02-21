using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class KnotNodeViewModel : BaseNodeViewModel
{
    
    [ObservableProperty] private ConnectorViewModelBase _connector = new();
}