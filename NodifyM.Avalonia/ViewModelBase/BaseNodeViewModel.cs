using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace NodifyM.Avalonia.ViewModelBase;

public partial class BaseNodeViewModel : ObservableObject, INodePosition
{
    [ObservableProperty] private Point _location;
}
