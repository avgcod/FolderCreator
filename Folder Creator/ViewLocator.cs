using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Folder_Creator.ViewModels;
using System;

namespace Folder_Creator
{
    public class ViewLocator : IDataTemplate
    {
        Control? ITemplate<object?, Control?>.Build(object? param)
        {
            var name = param?.GetType().FullName!.Replace("ViewModel", "View");
            Type? type;
            if (name != null)
            {
                type = Type.GetType(name);

                if (type != null)
                {
                    return (Control)Activator.CreateInstance(type)!;
                }
            }

            return new TextBlock { Text = "Not Found: " + name };


        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
