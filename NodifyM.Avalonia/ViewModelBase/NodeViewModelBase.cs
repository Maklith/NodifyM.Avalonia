using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class NodeViewModelBase : BaseNodeViewModel
{

    [ObservableProperty] private string _title="";
    [ObservableProperty] private string _footer="";
    [ObservableProperty] private ObservableCollection<object> _input = new();
    [ObservableProperty] private ObservableCollection<object> _output = new();
}