using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Folder_Creator.Views
{
    public partial class InformationView : Window
    {
        public InformationView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
