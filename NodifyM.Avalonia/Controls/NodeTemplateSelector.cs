// Author: liaom
// SolutionName: Kitopia
// ProjectName: NodifyM.Avalonia
// FileName:NodeTempleteSelector.cs
// Date: 2025/09/27 15:09
// FileEffect:

using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace NodifyM.Avalonia.Controls;

public class NodeTemplateSelector : IDataTemplate
{
    [Content] public DataTemplates? Templates { get; set; } = new DataTemplates();

    public Control? Build(object? param)
    {
        if (Templates != null)
        {
            foreach (var dataTemplate in Templates)
            {
                if (dataTemplate.Match(param))
                {
                    return dataTemplate.Build(param);
                }
            }
        }

        return null;
    }

    public bool Match(object? data)
    {
        return true;
    }
}