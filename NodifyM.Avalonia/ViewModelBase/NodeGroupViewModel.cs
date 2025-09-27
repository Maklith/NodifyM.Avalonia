// Author: liaom
// SolutionName: NodifyM.Avalonia
// ProjectName: NodifyM.Avalonia
// FileName:NodeGroupViewModel.cs
// Date: 2025/09/27 15:09
// FileEffect:

using Avalonia;

namespace NodifyM.Avalonia.ViewModelBase;

public class NodeGroupViewModel
{
    public object Header { get; set; } = "Group";
    public Size Size { get; set; } = new Size(200, 200);
    public bool CanResize { get; set; } = true;
}