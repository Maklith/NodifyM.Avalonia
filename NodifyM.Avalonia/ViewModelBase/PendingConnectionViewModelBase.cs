using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NodifyM.Avalonia.ViewModelBase;


public partial class PendingConnectionViewModelBase : ObservableObject
{
    private string[] _previewTargetNames = ["Can't connect itself", "Connect","Drop Connect"];
    
    [ObservableProperty] private object? _previewTarget;
    [ObservableProperty] private string? _previewText;
    [ObservableProperty] private ConnectorViewModelBase? _source;
    private readonly NodifyEditorViewModelBase _editor;

    /// <inheritdoc/>
    public PendingConnectionViewModelBase(NodifyEditorViewModelBase editor)
    {
        _editor = editor;
    }

    partial void OnPreviewTargetChanged(object? value)
    {
        switch (value)
        {
            case ConnectorViewModelBase con:
            {
                if (con == Source)
                {
                    PreviewText = _previewTargetNames[0];
                    break;
                }
                
                

                PreviewText = _previewTargetNames[1];

                break;
            }
            default:
                PreviewText = _previewTargetNames[2];
                break;
        }
    }

    [RelayCommand]
    public void Start(ConnectorViewModelBase item) => Source = item;

    [RelayCommand]
    public void Finish(ConnectorViewModelBase? target)
    {
        if (target == null|| Source ==null)
        {
            return;
        }

        if (target == Source)
        {
            return;
        }

        _editor.Connect(Source, target);
    }
}